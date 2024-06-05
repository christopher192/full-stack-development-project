import React, {useState, useEffect }from 'react';
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
import styles from './Index.module.css';


import ShiftList from './ShiftList';
import FullCalendar from '@fullcalendar/react' 
import dayGridPlugin from '@fullcalendar/daygrid' 
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';


// import Components

const Shifts = () => {
    document.title="Shifts | Digital Counter Dashboard";

    const [perPage, setPerPage] = useState(10);
	const [page, setPage] = useState(1);
	const [sortCol, setSortCol] = useState("id");
	const [sortDir, setSortDir] = useState("asc");
	const [searchValue, setSearchValue] = useState("");

    const [shift, setShift] = useState({});

    const [isEdit, setIsEdit] = useState(false);
    const [isDelete, setIsDelete] = useState(false);
    const [modal, setModal] = useState(false);
    const [deleteShiftData, setDeleteShiftData] = useState({});

    const GET_GROUPSHIFT = gql`
    query CombinedQuery($groupShiftApiRequest: ApiRequestInput!, $groupApiRequest: ApiRequestInput!, $shiftApiRequest: ApiRequestInput!) {
        groupShiftTable(apiRequest: $groupShiftApiRequest) {
        result {
            id
            shift_id
            group_id
            date
            status
        }
        total
        }
        groupTable(apiRequest: $groupApiRequest) {
        result {
            id
            group_name
            date_created
            status
        }
        total
        }
        shiftTable(apiRequest: $shiftApiRequest) {
        result {
            id
            shift_name
            start_time
            end_time
            status
        }
        total
        }
    }
    `;


    const CREATE_GROUPSHIFT = gql`
    mutation CreateGroupShift($appDcGroupShiftDTO: AppDcGroupShiftDTOInput!) {
        addGroupShift(appDcGroupShiftDTO: $appDcGroupShiftDTO) {
            result {
                id,
                group_id,
                shift_id,
                date,
                status,
            },
            status
        }
    }
`;

    const DELETE_GROUPSHIFT = gql`
        mutation DeleteGroupShift($appDcGroupShiftDTO: AppDcGroupShiftDTOInput!) {
            deleteGroupShift(appDcGroupShiftDTO: $appDcGroupShiftDTO) {
                result {
                    id,
                    group_id,
                    shift_id,
                    date,
                    status,
                },
                status
            }
        }
    `;


    const [createShift, { createData, createLoading, createError }] = useMutation(CREATE_GROUPSHIFT);
    const [deleteShift, { deleteData, deleteLoading, deleteError }] = useMutation(DELETE_GROUPSHIFT);


    const { loading, error, data, refetch} = useQuery(GET_GROUPSHIFT, {
        variables: {  
            groupShiftApiRequest: {
                page: page,
                perPage: perPage,
                sortCol: sortCol,
                sortDir: sortDir
              },
            groupApiRequest: {
                page: page,
                perPage: perPage,
                sortCol: sortCol,
                sortDir: sortDir
              },
            shiftApiRequest: {
                page: page,
                perPage: perPage,
                sortCol: sortCol,
                sortDir: sortDir
            }

        },
        fetchPolicy: "no-cache" 
    });

    useEffect(() => {  
        console.log(data);
    }, [data]);


    const handleCreateShiftClick = () => {
        setShift({});
        toggle();
    };

    const handleDeleteShiftSubmitClick = () => {
        const newDeleteShift = {
            id: parseInt(deleteShiftData._def.publicId),
        }
        
        deleteShift({ 
            
            variables: { appDcGroupShiftDTO: newDeleteShift }, 
               
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
        setDeleteShiftData({});
        setIsDelete(false); 
    };

    const handleDeleteShiftCancelClick = (event) => {
        setDeleteShiftData({event});
        setIsDelete(false); 
    };

    const handleDeleteShiftClick = (event) => {
        // const id = parseInt(event.id, 10);
        // const convertedEvent = { ...event, id };
        console.log(event)
        setDeleteShiftData(event);
        setIsDelete(true); 
    };

    const validation = useFormik({
        enableReinitialize: true,
        initialValues: {
            id: (shift && shift.id) || 0,
            group_id: (shift && shift.group_id) || '',
            shift_id: (shift && shift.shift_id) || '',
            date: (shift && shift.date) || '',
            status: (shift && shift.status) || 1
        },
        validationSchema: Yup.object({
           
        }),
        onSubmit: (value) => {
           
                const newShift = {
                    id: value.id,
                    shift_id: parseInt(value.shift_id),
                    group_id: parseInt(value.group_id),
                    date: value.date,
                    status: parseInt(value.status)
                }

                createShift({ 
                    variables: { appDcGroupShiftDTO: newShift }, 
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

    const closeModal = () => {
        validation.resetForm();
    };
    
    const EventComponent = ({ event }) => {

        let eventClassName = '';

        switch (event.title) {
        case 'Group A':
        eventClassName =  styles['event-group-a'];
        break;
        case 'Group B':
        eventClassName = styles['event-group-b'];
        break;
        case 'Group C':
        eventClassName = styles['event-group-c'];
        break;
        default:
        break;
        }

    return (
        <div className={`d-flex align-items-center justify-content-end ${eventClassName}`}>
            {event.title}
            <button className="btn" onClick={() => handleDeleteShiftClick(event)}>
                <i className="ri-close-circle-fill align-bottom" style={{color: "red"}}></i>
            </button>          
        </div>
    );
    };

    const getShiftTime = (shiftId) => {
        const shift = data?.shiftTable.result.find((shift) => shift.id === shiftId);
        return {
          start: shift.start_time,
          end: shift.end_time,
        };
    };

    const events = data?.groupShiftTable.result.map((groupShift) => {
        const group = data?.groupTable.result.find((group) => group.id === groupShift.group_id);
        const shiftTime = getShiftTime(groupShift.shift_id);

        let startDateTime = moment(`${groupShift.date} ${shiftTime.start}`, 'YYYY-MM-DD HH:mm');
        let endDateTime = moment(`${groupShift.date} ${shiftTime.end}`, 'YYYY-MM-DD HH:mm');

        if (groupShift.shift_id === 3) {
            // Shift is Night shift and end time is on the next day
            endDateTime = endDateTime.add(1, 'day');
        }

        return {
          id: groupShift.id,
          title: `${group.group_name}`,
          start: startDateTime.toISOString(),
          end: endDateTime.toISOString(),
        };
    });

    return (
        <React.Fragment>
             <ToastContainer closeButton={false} />
            <DeleteModal
                show={isDelete}
                onDeleteClick={() => handleDeleteShiftSubmitClick() }
                onCloseClick={() => handleDeleteShiftCancelClick() }
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
                    {"Create Shift"}
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
                                    <Label htmlFor="Group" className="form-label">Group</Label>
                                    <Input
                                        name="group_id"
                                        id="group_id"
                                        className="form-control"
                                        placeholder="Choose Group"
                                        type="select"
                                        validate={{
                                            required: { value: true },
                                        }}
                                        onChange={validation.handleChange}
                                        onBlur={validation.handleBlur}
                                        value={validation.values.group_id || ""}
                                        invalid={
                                            validation.touched.group_id && validation.errors.group_id ? true : false
                                        }
                                    >
                                        <option>Choose Group</option>
                                        {data?.groupTable.result.map((data) => (
                                        <option key={data.id} value={data.id}>
                                            {data.group_name}
                                        </option>
                                        ))}
                                    </Input>
                                    {validation.touched.group_id && validation.errors.group_id ? (
                                        <FormFeedback type="invalid">{validation.errors.group_id}</FormFeedback>
                                    ) : null}

                                    <Label htmlFor="shift_id" className="form-label">Shift</Label>
                                    <Input
                                        name="shift_id"
                                        id="shift_id"
                                        className="form-control"
                                        placeholder="Choose Shift"
                                        type="select"
                                        validate={{
                                            required: { value: true },
                                        }}
                                        onChange={validation.handleChange}
                                        onBlur={validation.handleBlur}
                                        value={validation.values.shift_id || ""}
                                        invalid={
                                            validation.touched.shift_id && validation.errors.shift_id ? true : false
                                        }
                                    >
                                        <option>Choose Shift</option>
                                        {data?.shiftTable.result.map((data) => (
                                        <option key={data.id} value={data.id}>
                                            {data.shift_name}
                                        </option>
                                        ))}
                                    </Input>
                                    {validation.touched.shift_id && validation.errors.shift_id ? (
                                        <FormFeedback type="invalid">{validation.errors.shift_id}</FormFeedback>
                                    ) : null}

                                    <Input
                                        name="status"
                                        id="status"
                                        className="form-control"
                                        placeholder="Enter Status"
                                        type="hidden"
                                        validate={{
                                            required: { value: true },
                                        }}
                                        onChange={validation.handleChange}
                                        onBlur={validation.handleBlur}
                                        value={1}
                                    />
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
                    <BreadCrumb title="Shifts" />
                    <Card>
                        <CardHeader>
                            <div className="d-flex align-items-center justify-content-end">
                                <div className="flex-shrink-0">
                                    <button className="btn btn-success add-btn" onClick={() => { 
                                        handleCreateShiftClick();
                                    }}>
                                        <i className="ri-add-line align-bottom"></i> Create Shift
                                    </button>
                                </div>
                            </div>
                        </CardHeader>
                        <CardBody>
                            <FullCalendar
                            plugins={[ dayGridPlugin,timeGridPlugin,interactionPlugin ]}
                            initialView="timeGridWeek"
                            events= {events}
                            eventContent={EventComponent}
                            />
                        </CardBody>
                    </Card>
                    
                    {/* <Card>
                        <CardHeader>
                            <h2>Set Group Shift</h2>
                        </CardHeader>
                        <CardBody>
                        <form>

                            <Row className="form-group mb-3">
                                <Col xxl={3} md={6}>
                                    <div>
                                        <Label htmlFor="exampleInputdate" className="form-label">Date:</Label>
                                        <Input type="date" className="form-control" id="exampleInputdate" />
                                    </div>
                                </Col>
                            </Row>
                            <Row className="form-group mb-3">
                                <Label htmlFor="shift" className="form-label">Shift:</Label>
                                <Col xxl={3} md={4}>
                                    <div className="input-group">
                                        <Label className="input-group-text" htmlFor="morningShift">Morning</Label>
                                        <select className="form-select" id="morningShift">
                                            <option >Choose Group</option>
                                            <option defaultValue="A">Group A</option>
                                            <option defaultValue="B">Group B</option>
                                            <option defaultValue="C">Group C</option>
                                        </select>
                                    </div>
                                </Col>
                                <Col xxl={3} md={4}>
                                    <div className="input-group">
                                        <Label className="input-group-text" htmlFor="afternoonShift">Afternoon</Label>
                                        <select className="form-select" id="afternoonShift">
                                            <option >Choose Group</option>
                                            <option defaultValue="A">Group A</option>
                                            <option defaultValue="B">Group B</option>
                                            <option defaultValue="C">Group C</option>
                                        </select>
                                    </div>
                                </Col>
                                <Col xxl={3} md={4}>
                                    <div className="input-group">
                                        <Label className="input-group-text" htmlFor="nightShift">Night</Label>
                                        <select className="form-select" id="nightShift">
                                            <option >Choose Group</option>
                                            <option defaultValue="A">Group A</option>
                                            <option defaultValue="B">Group B</option>
                                            <option defaultValue="C">Group C</option>
                                        </select>
                                    </div>
                                </Col>
                            </Row>
                            
                            <Row className="form-group mb-3">
                                <Col xxl={3} md={6} className="d-flex justify-content-end">
                                    <div className="input-group">
                                    <button className="btn btn-success mr-2" type="submit">Confirm</button>
                                    <button className="btn btn-danger" type="button">Cancel</button>
                                    </div>
                                </Col>
                            </Row>
                           
                        </form>
                        </CardBody>
                    </Card> */}
                    
                    {/* <ShiftList/> */}

                </Container>
            </div>
        
        </React.Fragment>
    );
};

export default Shifts;