import React, { useState, useEffect, useCallback } from 'react';
import DataTable from 'react-data-table-component';
import { Card, CardBody, CardHeader, Col, Container, Row, Modal, ModalBody, ModalHeader, ModalFooter, Label, FormFeedback, Form } from "reactstrap";
import { DropdownItem, DropdownMenu, DropdownToggle, Input, UncontrolledDropdown } from 'reactstrap';
import { useSelector, useDispatch } from "react-redux";
import { getApiListsReactTable, addApiList, updateApiList, deleteApiList } from "../../store/actions";
import * as Yup from "yup";
import { useFormik } from "formik";
import 'react-toastify/dist/ReactToastify.css';
import DeleteModal from "../../Components/Common/DeleteModal";
import { ToastContainer } from "react-toastify";

//Import Breadcrumb
import BreadCrumb from "../../Components/Common/BreadCrumb";

const WebApi = () => {
    document.title = "WebApi | Sophic Dashboard Builder";

    const dispatch = useDispatch();

    const { apiLists, isLoading, totalRows } = useSelector(state => ({
        totalRows: state.ApiLists.total,
        isLoading: state.ApiLists.isLoading,
        apiLists: state.ApiLists.apiLists
    }));

    const [perPage, setPerPage] = useState(10);
	const [page, setPage] = useState(1);
	const [sortCol, setSortCol] = useState(1);
	const [sortDir, setSortDir] = useState("asc");
    const [searchNameValue, setSearchNameValue] = useState("");
    const [searchDescriptionValue, setSearchDescriptionValue] = useState("");
    const [searchURLValue, setSearchURLValue] = useState("");
	const [searchTypeValue, setSearchTypeValue] = useState("");
    const [searchTempNameValue, setSearchTempNameValue] = useState("");
    const [searchTempDescriptionValue, setSearchTempDescriptionValue] = useState("");
    const [searchTempURLValue, setSearchTempURLValue] = useState("");
	const [searchTempTypeValue, setSearchTempTypeValue] = useState("");

    const [modal, setModal] = useState(false);
    const [isEdit, setIsEdit] = useState(false);
    const [deleteModal, setDeleteModal] = useState(false);
    const [apiList, setApiList] = useState({});
    const [deleteApiListData, setDeleteApiListData] = useState({});

    const columns = [
        {
            name: 'Id',
            selector: row => row.id,
            sortable: true,
            sortField: 'id'
        },
        {
            name: 'Name',
            selector: row => row.name,
            sortable: true,
            sortField: 'name'
        },
        {
            name: 'Description',
            selector: row => row.description,
            sortable: true,
            sortField: 'description',
            cell: (row, index, column, id) => {
                if (row.description) {
                    return row.description;
                } else {
                    return 'Not Available';
                }
            },
        },
        {
            name: 'URL',
            selector: row => row.url,
            sortable: true,
            sortField: 'url'
        },
        {
            name: 'Type',
            selector: row => row.type,
            sortable: true,
            sortField: 'type',
            cell: (row, index, column, id) => {
                switch(row.type) {
                    case 'bar': {
                        return "Bar Chart";
                    }
                    case 'pie': {
                        return "Pie Chart";
                    }
                    case 'line': {
                        return "Line Chart";
                    }
                    case 'gauge': {
                        return "Gauge Chart";
                    }
                    default:
                        return "Unavailablex";
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
                                handleEditWebApiClick(data);
                            }}>
                                <i className="ri-pencil-fill align-bottom me-2 text-muted"></i>Edit
                            </DropdownItem>
                            <DropdownItem className='remove-item-btn' data-bs-toggle="modal" onClick={() => {
                                handleDeleteWebApiClick(data);
                            }}>
                                <i className="ri-delete-bin-fill align-bottom me-2 text-muted"></i>Delete
                            </DropdownItem>
                        </DropdownMenu>
                    </UncontrolledDropdown>
                );
            },
        },
    ];

    const chartTypes = [
        { label: "Default", value: "" },
        { label: "Bar Chart", value: "bar" },
        { label: "Pie Chart", value: "pie" },
        { label: "Line Chart", value: "line" },
        { label: "Gauge Chart", value: "gauge" }
    ];

    const onChangeInSelect = (event) => {
        setSearchTempTypeValue(event.target.value);
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

    const searchButton = () => {
        setSearchNameValue(searchTempNameValue);
        setSearchDescriptionValue(searchTempDescriptionValue);
        setSearchURLValue(searchTempURLValue);
        setSearchTypeValue(searchTempTypeValue);
    };

    const resetButton = () => {
        setSearchNameValue("");
        setSearchDescriptionValue("");
        setSearchURLValue("");
        setSearchTypeValue("");
        setSearchTempNameValue("");
        setSearchTempDescriptionValue("");
        setSearchTempURLValue("");
        setSearchTempTypeValue("");
    };

    const toggle = useCallback(() => {
        if (modal) {
            setModal(false);
        } else {
            setModal(true);
        }
    }, [modal]);

    const validation = useFormik({
        enableReinitialize: true,
        initialValues: {
            id: (apiList && apiList.id) || 0,
            webApiName: (apiList && apiList.name) || '',
            webApiDescription: (apiList && apiList.description) || '',
            webApiUrl: (apiList && apiList.url) || '',
            webApiType: (apiList && apiList.type) || '',
        },
        validationSchema: Yup.object({
            webApiName: Yup.string().required("Please Enter Web API Name"),
            webApiUrl: Yup.string().required("Please Enter Web API URL"),
            webApiType: Yup.string().required("Please Enter Web API Type"),
        }),
        onSubmit: (values) => {
            if (isEdit) {
                const updWebApi = {
                    id: values["id"],
                    name: values["webApiName"],
                    description: values["webApiDescription"],
                    url: values["webApiUrl"],
                    type: values["webApiType"]
                };
                dispatch(updateApiList(updWebApi));  
                validation.resetForm();
            } 
            else {
                const newWebApi = {
                    id: values["id"],
                    name: values["webApiName"],
                    description: values["webApiDescription"],
                    url: values["webApiUrl"],
                    type: values["webApiType"]
                };
                let data = newWebApi;
                let params = { page: page, perPage: perPage, sortCol: sortCol, sortDir: sortDir, searches: [{ columnId: 1, columnValue: searchNameValue }, { columnId: 2, columnValue: searchDescriptionValue }, { columnId: 3, columnValue: searchURLValue }, { columnId: 4, columnValue: searchTypeValue }] };
                dispatch(addApiList(data, params));  
                validation.resetForm();
            }
            toggle();
        },
    });

    const handleCreateWebApiClick = useCallback(() => {
        setApiList({});
        setIsEdit(false);
        toggle();
    }, [toggle]);

    const closeModal = () => {
        validation.resetForm();
    };

    const handleEditWebApiClick = useCallback((arg) => {
        const webApi = arg;
        setApiList({
            id: webApi["id"],
            name: webApi["name"],
            description: webApi["description"],
            url: webApi["url"],
            type: webApi["type"]
        });
        setIsEdit(true);
        toggle();
    }, [toggle]);

    const handleDeleteWebApiClick = (apiList) => {
        setDeleteApiListData(apiList);
        setDeleteModal(true); 
    };

    const handleDeleteWebApiModelSubmitClick = () => {
        dispatch(deleteApiList(deleteApiListData));
        setDeleteModal(false); 
        setDeleteApiListData({});
    };

    const handleDeleteWebApiModelCancelClick = () => {
        setDeleteModal(false); 
        setDeleteApiListData({});
    };

    useEffect(() => {
        dispatch(getApiListsReactTable({ page: page, perPage: perPage, sortCol: sortCol, sortDir: sortDir, searches: [{ columnId: 1, columnValue: searchNameValue }, { columnId: 2, columnValue: searchDescriptionValue }, { columnId: 3, columnValue: searchURLValue }, { columnId: 4, columnValue: searchTypeValue }] }));
    }, [dispatch, page, perPage, sortCol, sortDir, searchNameValue, searchDescriptionValue, searchURLValue, searchTypeValue]);
    
    return (
        <React.Fragment>
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
                    {isEdit ? "Edit Web API" : "Create Web API"}
                </ModalHeader>

                <Form onSubmit={(e) => {
                    e.preventDefault();
                    validation.handleSubmit();
                    return false;
                }}>
                    <ModalBody>
                        <Row className="g-3">
                            <Input name="id" id="id" type="hidden" />
                            <Col lg={12}>
                                <div>
                                    <Label htmlFor="webApiName" className="form-label">Web API Name</Label>
                                    <Input
                                        name="webApiName"
                                        id="webApiName"
                                        className="form-control"
                                        placeholder="Enter Web API Name"
                                        type="text"
                                        validate={{
                                            required: { value: true },
                                        }}
                                        onChange={validation.handleChange}
                                        onBlur={validation.handleBlur}
                                        value={validation.values.webApiName || ""}
                                        invalid={
                                            validation.touched.webApiName && validation.errors.webApiName ? true : false
                                        }
                                    />
                                    {validation.touched.webApiName && validation.errors.webApiName ? (
                                        <FormFeedback type="invalid">{validation.errors.webApiName}</FormFeedback>
                                    ) : null}
                                </div>
                            </Col>
                            <Col lg={12}>
                                <div>
                                    <Label htmlFor="webApiDescription" className="form-label">Web API Description</Label>
                                    <Input
                                        name="webApiDescription"
                                        id="webApiDescription"
                                        className="form-control"
                                        placeholder="Enter Web API Description"
                                        type="text"
                                        validate={{
                                            required: { value: true },
                                        }}
                                        onChange={validation.handleChange}
                                        onBlur={validation.handleBlur}
                                        value={validation.values.webApiDescription || ""}
                                        invalid={
                                            validation.touched.webApiDescription && validation.errors.webApiDescription ? true : false
                                        }
                                    />
                                    {validation.touched.webApiDescription && validation.errors.webApiDescription ? (
                                        <FormFeedback type="invalid">{validation.errors.webApiDescription}</FormFeedback>
                                    ) : null}
                                </div>
                            </Col>                            
                            <Col lg={12}>
                                <div>
                                    <Label htmlFor="webApiUrl" className="form-label">Web API URL</Label>
                                    <Input
                                        name="webApiUrl"
                                        id="webApiUrl"
                                        className="form-control"
                                        placeholder="Enter Web API URL"
                                        type="text"
                                        validate={{
                                            required: { value: true },
                                        }}
                                        onChange={validation.handleChange}
                                        onBlur={validation.handleBlur}
                                        value={validation.values.webApiUrl || ""}
                                        invalid={
                                            validation.touched.webApiUrl && validation.errors.webApiUrl ? true : false
                                        }
                                    />
                                    {validation.touched.webApiUrl && validation.errors.webApiUrl ? (
                                        <FormFeedback type="invalid">{validation.errors.webApiUrl}</FormFeedback>
                                    ) : null}
                                </div>
                            </Col>
                            <Col lg={12}>
                                <div>
                                    <Label htmlFor="webApiType" className="form-label">Web API Type</Label>
                                    <Input
                                        name="webApiType"
                                        id="webApiType"
                                        className="form-control"
                                        placeholder="Enter Web API Type"
                                        type="select"
                                        validate={{
                                            required: { value: true },
                                        }}
                                        onChange={validation.handleChange}
                                        onBlur={validation.handleBlur}
                                        value={validation.values.webApiType || ""}
                                        invalid={
                                            validation.touched.webApiType && validation.errors.webApiType ? true : false
                                        }
                                    >
                                        {chartTypes.map((data) => (
                                            <option key={data.value} label={data.label} value={data.value}></option>
                                        ))}
                                    </Input>
                                    {validation.touched.webApiType && validation.errors.webApiType ? (
                                        <FormFeedback type="invalid">{validation.errors.webApiType}</FormFeedback>
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
            <ToastContainer closeButton={false} />
            <DeleteModal
                show={deleteModal}
                onDeleteClick={() => handleDeleteWebApiModelSubmitClick() }
                onCloseClick={() => handleDeleteWebApiModelCancelClick() }
            />    
            <div className="page-content">
                <Container fluid>
                    <BreadCrumb title="Web API List" pageTitle="Setting" />
                    <Row>
                        <Col>
                            <Card>
                                <CardHeader>
                                    <div className="d-flex align-items-center justify-content-end">
                                        <div className="flex-shrink-0">
                                            <button className="btn btn-success add-btn" onClick={() => { handleCreateWebApiClick(); }}><i className="ri-add-line align-bottom"></i> Create Web API</button>
                                        </div>
                                    </div>
                                </CardHeader>
                                <CardBody className="border border-dashed border-top-0 border-end-0 border-start-0">
                                    <form>
                                        <Row className="g-3 mb-3">
                                            <Col md={4}>
                                                <div className="search-box">
                                                    <input
                                                        id="search-bar-0"
                                                        type="text"
                                                        className="form-control search"
                                                        placeholder="Search For Name"
                                                        value={searchTempNameValue || ""}
                                                        onChange={(event) => { setSearchTempNameValue(event.target.value); }}
                                                    />
                                                    <i className="bx bx-search-alt search-icon"></i>
                                                </div>
                                            </Col>                                            
                                            <Col md={4}>
                                                <div className="search-box">
                                                    <input
                                                        id="search-bar-0"
                                                        type="text"
                                                        className="form-control search"
                                                        placeholder="Search For URL"
                                                        value={searchTempURLValue || ""}
                                                        onChange={(event) => { setSearchTempURLValue(event.target.value); }}
                                                    />
                                                    <i className="bx bx-search-alt search-icon"></i>
                                                </div>
                                            </Col>
                                            <Col md={4}>
                                                <div className="search-box">
                                                    <input
                                                        id="search-bar-0"
                                                        type="text"
                                                        className="form-control search"
                                                        placeholder="Search For Description"
                                                        value={searchTempDescriptionValue || ""}
                                                        onChange={(event) => { setSearchTempDescriptionValue(event.target.value); }}
                                                    />
                                                    <i className="bx bx-search-alt search-icon"></i>
                                                </div>
                                            </Col>                                           
                                        </Row>
                                        <Row className="g-3 mb-3">
                                            <Col md={4}>
                                                <select
                                                    className="form-select"
                                                    value={searchTempTypeValue}
                                                    onChange={onChangeInSelect}
                                                    >
                                                    {chartTypes.map((data) => (
                                                        <option key={data.value} label={data.label} value={data.value}></option>
                                                    ))}
                                                </select>
                                            </Col>                                            
                                        </Row>                                                    
                                        <Row>
                                            <Col md={3}>
                                                <div>
                                                    <button
                                                        type="button"
                                                        className="btn btn-primary w-100"
                                                        onClick={searchButton}
                                                    >
                                                        {" "}
                                                        <i className="ri-equalizer-fill me-2 align-bottom"></i>
                                                        Filters
                                                    </button>
                                                </div>
                                            </Col>
                                            <Col md={3}>
                                                <div>
                                                    <button
                                                        type="button"
                                                        className="btn btn-light w-100"
                                                        onClick={resetButton}
                                                    >
                                                        {" "}
                                                        <i className="ri-equalizer-fill me-2 align-bottom"></i>
                                                        Reset
                                                    </button>
                                                </div>
                                            </Col>                                        
                                        </Row>                                        
                                    </form>
                                </CardBody>
                                <CardBody>
                                    <DataTable
                                        columns={columns}
                                        data={apiLists}
                                        progressPending={isLoading}
                                        pagination
                                        paginationServer
                                        paginationTotalRows={totalRows}
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

export default WebApi;