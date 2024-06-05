import React, { useState, useEffect, useCallback } from 'react';
import { Card, CardBody, CardHeader, Col, Container, Row, DropdownItem, DropdownMenu, DropdownToggle, Input, UncontrolledDropdown } from 'reactstrap';
import { Modal, ModalBody, ModalHeader, ModalFooter, FormFeedback, Label, Form } from 'reactstrap';
import { useQuery, useLazyQuery, useMutation, gql } from '@apollo/client';
import DataTable from 'react-data-table-component';
import BreadCrumb from '../../Components/Common/BreadCrumb';
import * as Yup from "yup";
import { useFormik } from "formik";
import * as moment from "moment";
import DeleteModal from "../../Components/Common/DeleteModal";
import { ToastContainer } from 'react-toastify';


// import Components



const Target = () => {
    document.title="Target | Digital Counter Dashboard";
    const [perPage, setPerPage] = useState(10);
	const [page, setPage] = useState(1);
	const [sortCol, setSortCol] = useState("id");
	const [sortDir, setSortDir] = useState("asc");

    const [perPage2, setPerPage2] = useState(10);
	const [page2, setPage2] = useState(1);
	const [sortCol2, setSortCol2] = useState("date");
	const [sortDir2, setSortDir2] = useState("desc");

    const [modal, setModal] = useState(false);
    const [isEdit, setIsEdit] = useState(false);
    const [isDelete, setIsDelete] = useState(false);
    const [deleteTargetData, setDeleteTargetData] = useState({});

    const [target, setTarget] = useState({});
    const [selectedMachine, setSelectedMachine] = useState('');


    const GET_MACHINETARGET = gql`
    query CombinedQuery($machineApiRequest: ApiRequestInput!, $targetApiRequest: ApiRequestInput!) {
        machineTable(apiRequest: $machineApiRequest) {
        result {
            id
            machine_name
            dimension
            date_created
            status
            last_updated
        }
        total
        }
        targetTable(apiRequest: $targetApiRequest) {
        result {
            id
            machine_id
            target_morning
            target_afternoon
            target_night
            date
        }
        total
        }
    }
    `;

    const CREATE_TARGET = gql`
        mutation CreateTarget($appDcTargetDTO: AppDcTargetDTOInput!) {
            addTarget(appDcTargetDTO: $appDcTargetDTO) {
                result {
                    id,
                    machine_id,
                    target_morning,
                    target_afternoon,
                    target_night,
                    date
                },
                status
            }
        }
    `;

    const EDIT_TARGET = gql`
        mutation EditTarget($appDcTargetDTO: AppDcTargetDTOInput!) {
            editTarget(appDcTargetDTO: $appDcTargetDTO) {
                result {
                    id,
                    machine_id,
                    date,
                    target_morning,
                    target_afternoon,
                    target_night,
                },
                status
            }
        }
    `;

    const DELETE_TARGET = gql`
        mutation DeleteTarget($appDcTargetDTO: AppDcTargetDTOInput!) {
            deleteTarget(appDcTargetDTO: $appDcTargetDTO) {
                result {
                    id,
                    machine_id,
                    date,
                    target_morning,
                    target_afternoon,
                    target_night,
                },
                status
            }
        }
    `;
    
    const [createTarget, { createData, createLoading, createError }] = useMutation(CREATE_TARGET);
    const [editTarget, { editData, editLoading, editError }] = useMutation(EDIT_TARGET);
    const [deleteTarget, { deleteData, deleteLoading, deleteError }] = useMutation(DELETE_TARGET);

    const { loading, error, data, refetch} = useQuery(GET_MACHINETARGET, {
        variables: {  
            machineApiRequest: {
                page: page,
                perPage: perPage,
                sortCol: sortCol,
                sortDir: sortDir
              },
            targetApiRequest: {
                page: page2,
                perPage: perPage2,
                sortCol: sortCol2,
                sortDir: sortDir2
              }
        },
        fetchPolicy: "no-cache" 
    });

    const getMachineName = (machineId) => {
        const machine = data?.machineTable.result.find(machine => machine.id === machineId);
        return machine ? machine.machine_name : 'Not Available';
    };

    const columns = [
        {
            name: 'Machine',
            selector: row => getMachineName(row.machine_id),
            sortable: true,
            sortField: 'machine_id'
        },
        {
            name: 'Date',
            selector: row => (new Date(row.date)).toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit', year: 'numeric' }),
            sortable: true,
            sortField: 'date',
            cell: (row, index, column, id) => {
                if (!row.date) {
                    return "Not Available";
                } else {
                    return moment(new Date(row.date)).format("DD MMM Y, h:mm A");
                }
            },
        },
        {
            name: 'Target Morning',
            selector: row => row.target_morning,
            sortable: true,
            sortField: 'target_morning'
        },
        {
            name: 'Target Afternoon',
            selector: row => row.target_afternoon,
            sortable: true,
            sortField: 'target_afternoon'
        },
        {
            name: 'Target Night',
            selector: row => row.target_night,
            sortable: true,
            sortField: 'target_night'
        },
        {
            name: <span className='font-weight-bold fs-13'>Action</span>,
            cell: (data) => {
                return (
                    <UncontrolledDropdown className="dropdown d-inline-block">
                        <DropdownToggle className="btn btn-soft-secondary btn-sm" tag="button">
                            <i className="ri-more-fill align-middle"></i>
                        </DropdownToggle>
                        <DropdownMenu className="dropdown-menu-end">
                            <DropdownItem className='edit-item-btn' data-bs-toggle="modal" onClick={() => {
                                handleEditTargetClick(data);
                            }}>
                                <i className="ri-pencil-fill align-bottom me-2 text-muted"></i>Edit
                            </DropdownItem>
                            <DropdownItem className='remove-item-btn' data-bs-toggle="modal" onClick={() => {
                                handleDeleteTargetClick(data);
                            }}>
                                <i className="ri-delete-bin-fill align-bottom me-2 text-muted"></i>Delete
                            </DropdownItem>
                        </DropdownMenu>
                    </UncontrolledDropdown>
                );
            },
        },
    ];

    const validation = useFormik({
        enableReinitialize: true,
        initialValues: {
            id: (target && target.id) || 0,
            machine_id: (target && target.machine_id) || '',
            date: (target && target.date) || '',
            target_morning: (target && target.target_morning) || '',
            target_afternoon: (target && target.target_afternoon) || '',
            target_night: (target && target.target_night) || ''
        },
        validationSchema: Yup.object({
            machine_id: Yup.string().required("Please Choose a Machine"),
            date: Yup.string().required("Please Enter Date"),
            target_morning: Yup.string().required("Please Enter Target Morning"),
            target_afternoon: Yup.string().required("Please Enter Target Afternoon"),
            target_night: Yup.string().required("Please Enter Target Night"),
        }),
        onSubmit: (value) => {
            if (isEdit) {
                const uptTarget = {
                    id: value.id,
                    machine_id: parseInt(value.machine_id),
                    date: value.date,
                    target_morning: parseInt(value.target_morning),
                    target_afternoon: parseInt(value.target_afternoon),
                    target_night: parseInt(value.target_night)
                };

                editTarget({ 
                    variables: { appDcTargetDTO: uptTarget }, 
                    onCompleted(data) { 
                        // if (data.addMachine.status === 1) {

                        // }
                        refetch({
                            apiRequest: {
                                "page": page,
                                "perPage": perPage
                            }
                        });
                    },
                    onError(error) {

                    }
                });

                validation.resetForm();
            } 
            else {
                const newTarget = {
                    id: value.id,
                    machine_id: parseInt(value.machine_id),
                    date: value.date,
                    target_morning: parseInt(value.target_morning),
                    target_afternoon: parseInt(value.target_afternoon),
                    target_night: parseInt(value.target_night)
                }

                createTarget({ 
                    variables: { appDcTargetDTO: newTarget }, 
                    onCompleted(data) { 
                        // if (data.addMachine.status === 1) {

                        // }
                        refetch({
                            apiRequest: {
                                "page": page,
                                "perPage": perPage,
                                "sortCol" : sortCol,
                                "sortDir": sortDir
                            }
                        });
                    },
                    onError(error) {
                        console.log(error);
                    }
                });

                validation.resetForm();
            }
            
            toggle();
        },
    });

    const toggle = () => {
        if (modal) {
            setModal(false);
        } else {
            setModal(true);
        }
    };

    const handleCreateTargetClick = () => {
        setTarget({});
        setIsEdit(false);
        toggle();
    };

    const handlePerRowsChange = (newPerPage, cPage) => {
        setPerPage2(newPerPage);
        setPage2(cPage);
	};

    const handlePageChange = newPage => {
        setPage2(newPage);
	};

    const handleSort = (column, sortDirection) => {
        setSortCol2(column.sortField);
        setSortDir2(sortDirection);
    };

    const closeModal = () => {
        validation.resetForm();
    };

    const handleEditTargetClick = (arg) => {
        const target = arg;
        setTarget({
            id:  target["id"],
            machine_id: target["machine_id"],
            date_created: moment(new Date(target["date_created"])).format("yyyy-MM-DDThh:mm"),
            target_morning: target["target_morning"],
            target_afternoon: target["target_afternoon"],
            target_night: target["target_night"]
        });
        setIsEdit(true);
        toggle();
    };

    const handleDeleteTargetClick = (data) => {
        setDeleteTargetData(data);
        setIsDelete(true); 
    };

    const handleDeleteTargetSubmitClick = () => {
        deleteTarget({ 
            variables: { appDcTargetDTO: deleteTargetData }, 
            onCompleted(data) { 
                // if (data.addMachine.status === 1) {

                // }
                refetch({
                    apiRequest: {
                        "page": page,
                        "perPage": perPage,
                        "sortCol" : sortCol,
                        "sortDir": sortDir
                    }
                });
            },
            onError(error) {

            }
        });
        setDeleteTargetData({});
        setIsDelete(false); 
    };

    const handleDeleteTargetCancelClick = () => {
        setDeleteTargetData({});
        setIsDelete(false); 
    };


    const handleMachineButtonClick = (machineId) => {
        setSelectedMachine(machineId);
    };

    const filteredData = selectedMachine
    ? data?.targetTable.result.filter(row => row.machine_id === selectedMachine)
    : data?.targetTable.result;

    useEffect(() => {  
        console.log(data);
        //console.log(data1);
    }, [data]);

    return (
        <React.Fragment>
            <ToastContainer closeButton={false} />
            <DeleteModal
                show={isDelete}
                onDeleteClick={() => handleDeleteTargetSubmitClick() }
                onCloseClick={() => handleDeleteTargetCancelClick() }
            />    
            <Modal
                isOpen={modal}
                toggle={toggle}
                centered
                size="lg"
                className="border-0"
                modalClassName="zoomIn"
                onClosed={closeModal} 
            >
                <ModalHeader className="p-3 bg-soft-info">
                    {isEdit ? "Edit Target" : "Create Target"}
                </ModalHeader>

                <Form onSubmit={(e) => {
                    e.preventDefault();
                    validation.handleSubmit();
                    return false;
                }}>
                    <ModalBody>
                        <Row className="g-3">
                            <Col lg={12}>
                                <div>
                                    <Label htmlFor="machine_id" className="form-label">Machine</Label>
                                    <Input
                                        name="machine_id"
                                        id="machine_id"
                                        className="form-control"
                                        placeholder="Choose Machine"
                                        type="select"
                                        validate={{
                                            required: { value: true },
                                        }}
                                        onChange={validation.handleChange}
                                        onBlur={validation.handleBlur}
                                        value={validation.values.machine_id || ""}
                                        invalid={
                                            validation.touched.machine_id && validation.errors.machine_id ? true : false
                                        }
                                    >
                                        <option>Choose Machine</option>
                                        {data?.machineTable.result.map((data) => (
                                        <option key={data.id} value={data.id}>
                                            {data.machine_name}
                                        </option>
                                        ))}
                                    </Input>
                                    {validation.touched.machine_id && validation.errors.machine_id ? (
                                        <FormFeedback type="invalid">{validation.errors.machine_id}</FormFeedback>
                                    ) : null}
                                </div>
                            </Col>
                            <Col lg={12}>
                                <div>
                                    <Label htmlFor="date" className="form-label">Date:</Label>
                                    <Input type="datetime-local" className="form-control" name="date" id="date"
                                        validate={{
                                            required: { value: true },
                                        }}
                                        onChange={validation.handleChange}
                                        onBlur={validation.handleBlur}
                                        value={validation.values.date || ""}
                                        invalid={
                                            validation.touched.date && validation.errors.date? true : false
                                        }
                                    />
                                    {validation.touched.date && validation.errors.date ? (
                                        <FormFeedback type="invalid">{validation.errors.date}</FormFeedback>
                                    ) : null}
                                </div>
                            </Col>           
                            <Col lg={12}>
                                <div>
                                    <Label htmlFor="target_morning" className="form-label">Target Morning</Label>
                                    <Input
                                        name="target_morning"
                                        id="target_morning"
                                        className="form-control"
                                        placeholder="Enter Target"
                                        type="text"
                                        validate={{
                                            required: { value: true },
                                        }}
                                        onChange={validation.handleChange}
                                        onBlur={validation.handleBlur}
                                        value={validation.values.target_morning || ""}
                                        invalid={
                                            validation.touched.target_morning && validation.errors.target_morning ? true : false
                                        }
                                    />
                                    {validation.touched.target_morning && validation.errors.target_morning ? (
                                        <FormFeedback type="invalid">{validation.errors.target_morning}</FormFeedback>
                                    ) : null}
                                </div>
                            </Col>
                            <Col lg={12}>
                                <div>
                                    <Label htmlFor="target_afternoon" className="form-label">Target Afternoon</Label>
                                    <Input
                                        name="target_afternoon"
                                        id="target_afternoon"
                                        className="form-control"
                                        placeholder="Enter Target"
                                        type="text"
                                        validate={{
                                            required: { value: true },
                                        }}
                                        onChange={validation.handleChange}
                                        onBlur={validation.handleBlur}
                                        value={validation.values.target_afternoon || ""}
                                        invalid={
                                            validation.touched.target_afternoon && validation.errors.target_afternoon ? true : false
                                        }
                                    />
                                    {validation.touched.target_afternoon && validation.errors.target_afternoon ? (
                                        <FormFeedback type="invalid">{validation.errors.target_afternoon}</FormFeedback>
                                    ) : null}
                                </div>
                            </Col>
                            <Col lg={12}>
                                <div>
                                    <Label htmlFor="target_night" className="form-label">Target Night</Label>
                                    <Input
                                        name="target_night"
                                        id="target_night"
                                        className="form-control"
                                        placeholder="Enter Target"
                                        type="text"
                                        validate={{
                                            required: { value: true },
                                        }}
                                        onChange={validation.handleChange}
                                        onBlur={validation.handleBlur}
                                        value={validation.values.target_night || ""}
                                        invalid={
                                            validation.touched.target_night && validation.errors.target_night ? true : false
                                        }
                                    />
                                    {validation.touched.target_night && validation.errors.target_night ? (
                                        <FormFeedback type="invalid">{validation.errors.target_night}</FormFeedback>
                                    ) : null}
                                </div>
                            </Col>
                                                            
                        </Row>
                    </ModalBody>
                    <ModalFooter>
                        <div className="hstack gap-2 justify-content-end">
                            <button onClick={() => { setModal(false); setIsEdit(false); }} type="button" className="btn btn-light" data-bs-dismiss="modal">Close</button>
                            <button type="submit" className="btn btn-success" id="add-btn">{isEdit ? "Update" : "Create"}</button>
                        </div>
                    </ModalFooter>                          
                </Form>
            </Modal>
            <div className="page-content">
                
                <Container fluid>
                    <BreadCrumb title="Target"/>
                    <Row>
                        <Col lg={12}>
                            <Card>
                                <CardHeader>
                                    <div className="d-flex justify-content-between">
                                        <div>
                                        <button className="btn btn-warning" style={{ marginRight: '8px' }} onClick={() => handleMachineButtonClick(null)}>Show All</button>
                                        {data?.machineTable.result
                                            .sort((a, b) => a.machine_name.localeCompare(b.machine_name))
                                            .map((machine, index) => (
                                            <button
                                                className="btn btn-info"
                                                key={machine.id}
                                                onClick={() => handleMachineButtonClick(machine.id)}
                                                style={{ marginRight: index !== data.machineTable.result.length - 1 ? '8px' : '0' }}
                                            >
                                                {machine.machine_name}
                                            </button>
                                            ))}
                                        </div>
                                        <div className="flex-shrink-0">
                                            <button className="btn btn-success add-btn" onClick={handleCreateTargetClick}>
                                                <i className="ri-add-line align-bottom"></i> Create Target
                                            </button>
                                        </div>
                                    </div>
                                </CardHeader>
                                <CardBody>
                                
                                    <DataTable
                                        columns={columns}
                                        data={filteredData}
                                        progressPending={loading}
                                        pagination
                                        paginationServer
                                        paginationTotalRows={data?.targetTable.total}
                                        onChangeRowsPerPage={handlePerRowsChange}
                                        onChangePage={handlePageChange}
                                        onSort={handleSort}
                                        sortServer
                                        striped
                                        defaultSortFieldId={2}
                                        defaultSortAsc={false}
                                    />
                                </CardBody>
                            </Card>
                        </Col>
                    </Row>
                </Container>
            </div>
        </React.Fragment>

    );
};

export default Target;