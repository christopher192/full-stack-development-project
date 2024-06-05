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

const Machine = () => {
    document.title = "Machine | Digital Counter Dashboard";
    
    const [perPage, setPerPage] = useState(10);
	const [page, setPage] = useState(1);
	const [sortCol, setSortCol] = useState("machine_name");
	const [sortDir, setSortDir] = useState("asc");
	const [searchValue, setSearchValue] = useState("");

    const [machine, setMachine] = useState({});

    const [modal, setModal] = useState(false);
    const [isEdit, setIsEdit] = useState(false);
    const [isDelete, setIsDelete] = useState(false);
    const [deleteMachineData, setDeleteMachineData] = useState({});

    const GET_MACHINES = gql`
        query GetMachineTable($apiRequest: ApiRequestInput!) {
            machineTable(apiRequest: $apiRequest){
                result {
                    id,
                    machine_name,
                    dimension,
                    date_created,
                    status,
                    last_updated
                },
                total
            }
        }
    `;
    
    const CREATE_MACHINE = gql`
        mutation CreateMachine($appDcMachineDTO: AppDcMachineDTOInput!) {
            addMachine(appDcMachineDTO: $appDcMachineDTO) {
                result {
                    id,
                    machine_name,
                    dimension,
                    date_created,
                    status,
                    last_updated
                },
                status
            }
        }
    `;

    const EDIT_MACHINE = gql`
        mutation EditMachine($appDcMachineDTO: AppDcMachineDTOInput!) {
            editMachine(appDcMachineDTO: $appDcMachineDTO) {
                result {
                    id,
                    machine_name,
                    dimension,
                    date_created,
                    status,
                    last_updated
                },
                status
            }
        }
    `;

    const DELETE_MACHINE = gql`
        mutation DeleteMachine($appDcMachineDTO: AppDcMachineDTOInput!) {
            deleteMachine(appDcMachineDTO: $appDcMachineDTO) {
                result {
                    id,
                    machine_name,
                    dimension,
                    date_created,
                    status,
                    last_updated
                },
                status
            }
        }
    `;

    const [createMachine, { createData, createLoading, createError }] = useMutation(CREATE_MACHINE);
    const [editMachine, { editData, editLoading, editError }] = useMutation(EDIT_MACHINE);
    const [deleteMachine, { deleteData, deleteLoading, deleteError }] = useMutation(DELETE_MACHINE);

    const { loading, error, data, refetch } = useQuery(GET_MACHINES, {
        variables: { apiRequest: {
                "page": page,
                "perPage": perPage,
                "sortCol" : sortCol,
                "sortDir": sortDir
            }
        },
        fetchPolicy: "no-cache" 
    });

    const columns = [
        {
            name: 'Machine Name',
            selector: row => row.machine_name,
            sortable: true,
            sortField: 'machine_name'
        },
        {
            name: 'Dimension',
            selector: row => row.dimension,
            sortable: true,
            sortField: 'dimension'
        },
        {
            name: 'Created Date',
            selector: row => row.date_created,
            sortable: true,
            sortField: 'date_created',
            cell: (row, index, column, id) => {
                if (!row.date_created) {
                    return "Not Available";
                } else {
                    return moment(new Date(row.date_created)).format("DD MMM Y, h:mm A");
                }
            },
        },
        {
            name: 'Status',
            selector: row => row.status,
            sortable: true,
            sortField: 'status'
        },
        {
            name: 'Last Updated',
            selector: row => row.last_updated,
            sortable: true,
            sortField: 'last_updated',
            cell: (row, index, column, id) => {
                if (!row.last_updated) {
                    return "Not Available";
                } else {
                    return moment(new Date(row.last_updated)).format("DD MMM Y, h:mm A");
                }
            },
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
                            <DropdownItem>
                                <i className="ri-eye-fill align-bottom me-2 text-muted"></i>View
                            </DropdownItem>
                            <DropdownItem className='edit-item-btn' data-bs-toggle="modal" onClick={() => {
                                handleEditMachineClick(data);
                            }}>
                                <i className="ri-pencil-fill align-bottom me-2 text-muted"></i>Edit
                            </DropdownItem>
                            <DropdownItem className='remove-item-btn' data-bs-toggle="modal" onClick={() => {
                                handleDeleteMachineClick(data);
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
            id: (machine && machine.id) || 0,
            machineName: (machine && machine.machine_name) || '',
            dimension: (machine && machine.dimension) || '',
            dateCreated: (machine && machine.date_created) || '',
            machineStatus: (machine && machine.status) || 1,
            lastUpdated: (machine && machine.last_updated) || ''
        },
        validationSchema: Yup.object({
            machineName: Yup.string().required("Please Enter Machine Name"),
            dimension: Yup.string().required("Please Enter Dimension"),
            dateCreated: Yup.string().required("Please Enter Created Date"),
            machineStatus: Yup.string().required("Please Enter Status")
        }),
        onSubmit: (value) => {
            if (isEdit) {
                const uptMachine = {
                    id: value.id,
                    machine_name: value.machineName,
                    status: parseInt(value.machineStatus),
                    dimension: value.dimension,
                    date_created: value.dateCreated
                };

                editMachine({ 
                    variables: { appDcMachineDTO: uptMachine }, 
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

                validation.resetForm();
            } 
            else {
                const newMachine = {
                    id: value.id,
                    machine_name: value.machineName,
                    status: parseInt(value.machineStatus),
                    dimension: value.dimension,
                    date_created: value.dateCreated
                }

                createMachine({ 
                    variables: { appDcMachineDTO: newMachine }, 
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

    const handleCreateMachineClick = () => {
        setMachine({});
        setIsEdit(false);
        toggle();
    };

    const handlePerRowsChange = (newPerPage, cPage) => {
        setPerPage(newPerPage);
        setPage(cPage);
	};

    const handlePageChange = newPage => {
        setPage(newPage);
	};

    const handleSort = (column, sortDirection) => {
        setSortCol(column.sortField);
        setSortDir(sortDirection);
    };

    const closeModal = () => {
        validation.resetForm();
    };

    const handleEditMachineClick = (arg) => {
        const machine = arg;
        setMachine({
            id:  machine["id"],
            machine_name: machine["machine_name"],
            dimension: machine["dimension"],
            date_created: moment(new Date(machine["date_created"])).format("yyyy-MM-DDThh:mm"),
            status: machine["status"]
        });
        setIsEdit(true);
        toggle();
    };

    const handleDeleteMachineClick = (data) => {
        setDeleteMachineData(data);
        setIsDelete(true); 
    };

    const handleDeleteMachineSubmitClick = () => {
        deleteMachine({ 
            variables: { appDcMachineDTO: deleteMachineData }, 
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
        setDeleteMachineData({});
        setIsDelete(false); 
    };

    const handleDeleteMachineCancelClick = () => {
        setDeleteMachineData({});
        setIsDelete(false); 
    };

    useEffect(() => {  
        console.log(data);
    }, [data]);

    return (
        <React.Fragment>
            <ToastContainer closeButton={false} />
            <DeleteModal
                show={isDelete}
                onDeleteClick={() => handleDeleteMachineSubmitClick() }
                onCloseClick={() => handleDeleteMachineCancelClick() }
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
                    {isEdit ? "Edit Machine" : "Create Machine"}
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
                                    <Label htmlFor="machineName" className="form-label">Machine Name</Label>
                                    <Input
                                        name="machineName"
                                        id="machineName"
                                        className="form-control"
                                        placeholder="Enter Machine Name"
                                        type="text"
                                        validate={{
                                            required: { value: true },
                                        }}
                                        onChange={validation.handleChange}
                                        onBlur={validation.handleBlur}
                                        value={validation.values.machineName || ""}
                                        invalid={
                                            validation.touched.machineName && validation.errors.machineName ? true : false
                                        }
                                    />
                                    {validation.touched.machineName && validation.errors.machineName ? (
                                        <FormFeedback type="invalid">{validation.errors.machineName}</FormFeedback>
                                    ) : null}
                                </div>
                            </Col>
                            <Col lg={12}>
                                <div>
                                    <Label htmlFor="dimension" className="form-label">Dimension</Label>
                                    <Input
                                        name="dimension"
                                        id="dimension"
                                        className="form-control"
                                        placeholder="Enter Dimension"
                                        type="text"
                                        validate={{
                                            required: { value: true },
                                        }}
                                        onChange={validation.handleChange}
                                        onBlur={validation.handleBlur}
                                        value={validation.values.dimension || ""}
                                        invalid={
                                            validation.touched.dimension && validation.errors.dimension? true : false
                                        }
                                    />
                                    {validation.touched.dimension && validation.errors.dimension ? (
                                        <FormFeedback type="invalid">{validation.errors.dimension}</FormFeedback>
                                    ) : null}
                                </div>
                            </Col>
                            {isEdit && (<Col lg={12}>
                                <div>
                                    <Label htmlFor="machineStatus" className="form-label">Status</Label>
                                    <Input
                                        name="machineStatus"
                                        id="machineStatus"
                                        className="form-control"
                                        placeholder="Enter Status"
                                        type="select"
                                        validate={{
                                            required: { value: true },
                                        }}
                                        onChange={validation.handleChange}
                                        onBlur={validation.handleBlur}
                                        value={validation.values.machineStatus || ""}
                                        invalid={
                                            validation.touched.machineStatus && validation.errors.machineStatus? true : false
                                        }
                                    >
                                        <option>1</option>
                                        <option>2</option>
                                    </Input>
                                    {validation.touched.machineStatus && validation.errors.machineStatus ? (
                                        <FormFeedback type="invalid">{validation.errors.machineStatus}</FormFeedback>
                                    ) : null}
                                </div>
                            </Col>)}
                            <Col lg={12}>
                                <div>
                                    <Label htmlFor="dateCreated" className="form-label">Created Date:</Label>
                                    <Input type="datetime-local" className="form-control" name="dateCreated" id="dateCreated"
                                        validate={{
                                            required: { value: true },
                                        }}
                                        onChange={validation.handleChange}
                                        onBlur={validation.handleBlur}
                                        value={validation.values.dateCreated || ""}
                                        invalid={
                                            validation.touched.dateCreated && validation.errors.dateCreated? true : false
                                        }
                                    />
                                    {validation.touched.dateCreated && validation.errors.dateCreated ? (
                                        <FormFeedback type="invalid">{validation.errors.dateCreated}</FormFeedback>
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
                    <BreadCrumb title="List" pageTitle="Machine" />
                    <Row>
                        <Col lg={12}>
                            <Card>
                                <CardHeader>
                                    <div className="d-flex align-items-center justify-content-end">
                                        <div className="flex-shrink-0">
                                            <button className="btn btn-success add-btn" onClick={() => { 
                                                handleCreateMachineClick();
                                            }}>
                                                <i className="ri-add-line align-bottom"></i> Create Machine
                                            </button>
                                        </div>
                                    </div>
                                </CardHeader>
                                <CardBody>
                                    <DataTable
                                        columns={columns}
                                        data={data?.machineTable.result}
                                        progressPending={loading}
                                        pagination
                                        paginationServer
                                        paginationTotalRows={data?.machineTable.total}
                                        onChangeRowsPerPage={handlePerRowsChange}
                                        onChangePage={handlePageChange}
                                        onSort={handleSort}
                                        sortServer
                                        striped
                                        defaultSortFieldId={1}
                                        defaultSortAsc={true}
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

export default Machine;
