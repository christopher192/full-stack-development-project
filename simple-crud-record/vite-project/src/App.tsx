import React, { useState, useEffect } from 'react'
import { Container, Row, Col, Card, Button, Modal } from 'react-bootstrap';
import Form from 'react-bootstrap/Form';
import DataTable from 'react-data-table-component';
import axios from 'axios';
import moment from 'moment';

function App() {
  const columns = [
    {
      name: 'Id',
      selector: row => row.id,
      sortable: true,
      sortField: 'id'
    },
    {
      name: 'Region',
      selector: row => row.region,
      sortable: true,
      sortField: 'region'
    },
    {
      name: 'Country',
      selector: row => row.country,
      sortable: true,
      sortField: 'country'
    },
    {
      name: 'Item Type',
      selector: row => row.itemType,
      sortable: true,
      sortField: 'itemType'
    },
    {
      name: 'Order Date',
      selector: row => row.orderDate,
      sortable: true,
      sortField: 'orderDate'
    },
    {
      name: 'Order ID',
      selector: row => row.orderID
    },
    {
      name: 'Order Priority',
      selector: row => row.orderPriority
    },
    {
      name: 'Sales Channel',
      selector: row => row.salesChannel
    },
    {
      name: 'Ship Date',
      selector: row => row.shipDate,
      sortable: true,
      sortField: 'shipDate'
    },
    {
      name: 'Total Cost',
      selector: row => row.totalCost
    },
    {
      name: 'Total Profit',
      selector: row => row.totalProfit
    },
    {
      name: 'Total Revenue',
      selector: row => row.totalRevenue
    },
    {
      name: 'Unit Cost',
      selector: row => row.unitCost
    },
    {
      name: 'Unit Price',
      selector: row => row.unitPrice
    },
    {
      name: 'Units Sold',
      selector: row => row.unitsSold
    },
    {
      name: <span>Action</span>,
      cell: (data) => {
        return (
          <div>
            <button type = "button" className = "btn btn-primary" onClick = {() => handleCEShow(data, "edit")}>Edit</button>
            <button type = "button" className = "btn btn-danger" onClick = {() => handleShow(data.id)}>Delete</button>
          </div>
        );
      }
    }
  ];

  const [perPage, setPerPage] = useState(10);
	const [page, setPage] = useState(1);
	const [sortCol, setSortCol] = useState(1);
	const [sortDir, setSortDir] = useState("asc");
  const [recordList, setRecordList] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [totalRows, setTotalRows] = useState(0);
  const [region, setRegion] = useState('');
  const [country, setCountry] = useState('');
  const [searchRegionValue, setSearchRegionValue] = useState("");
  const [searchCountryValue, setSearchCountryValue] = useState("");

  const searchButton = () => {
    setSearchRegionValue(region);
    setSearchCountryValue(country);
  };

  const resetButton = () => {
    setRegion("");
    setSearchRegionValue("");
    setCountry("");
    setSearchCountryValue("");
};

  const fetchData = async (page, perPage, sortCol, sortDir, searchRegionValue, searchCountryValue) => {
    setIsLoading(true);
    try {
      const params = {
        page: page,
        perPage: perPage,
        sortCol: sortCol,
        sortDir: sortDir,
        searches: [
          {
            "columnId": 1,
            "columnValue": searchRegionValue
          },
          {
            "columnId": 2,
            "columnValue": searchCountryValue
          },
        ]
      };
      const response = await axios.post('https://localhost:44379/api/records/getreacttable', params);
      setRecordList(response.data.data);
      setTotalRows(response.data.total);
      setIsLoading(false);
    } catch (error) {
      console.error('error fetching data: ', error);
      setIsLoading(false);
    }
  };

  const createRecord = async (record) => {
    try {
      const response = await axios.post('https://localhost:44379/api/records', record);
      if (response.status === 201) {
        fetchData(page, perPage, sortCol, sortDir, searchRegionValue, searchCountryValue);
      } else {
        console.log('error creating record: ', response);
      }
    } catch (error) {
      console.error('error creating record: ', error);
    }
  };
  
  const updateRecord = async (id, updatedRecord) => {
    try {
      const response = await axios.put(`https://localhost:44379/api/records/${id}`, updatedRecord);
      if (response.status === 204) {
        fetchData(page, perPage, sortCol, sortDir, searchRegionValue, searchCountryValue);
      } else {
        console.log('error updating record: ', response);
      }
    } catch (error) {
      console.error('error updating record: ', error);
    }
  };

  const deleteRecord = async (id) => {
    try {
      const response = await axios.delete(`https://localhost:44379/api/records/${id}`);
      if (response.status === 204) {
        fetchData(page, perPage, sortCol, sortDir, searchRegionValue, searchCountryValue);
      } else {
        console.log('error deleting record: ', response);
      }
    } catch (error) {
      console.error('error deleting record: ', error);
    }
  };

  useEffect(() => {
    fetchData(page, perPage, sortCol, sortDir, searchRegionValue, searchCountryValue);
  }, [page, perPage, sortCol, sortDir, searchRegionValue, searchCountryValue]);

  const handlePerRowsChange = (newPerPage, cPage) => {
    setPerPage(newPerPage);
    setPage(cPage);
  };

  const handlePageChange = newPage => {
    setPage(newPage);
  };

  const handleSort = (column, sortDirection) => {
    setSortCol(column.id);
    setSortDir(sortDirection);
  };

  const [show, setShow] = useState(false);
  const [modalData, setModalData] = useState({ id: '', body: '' });

  const handleShow = (id) => {
    setModalData({ id: id, body: `Are you sure you want to delete this record (${id})?` });
    setShow(true);
  };

  const handleClose = () => {
    setShow(false);
    setModalData({ id: '', body: '' });
  };

  const handleDeleteSubmit = () => {
    setShow(false);
    deleteRecord(modalData.id);
    setModalData({ id: '', body: '' });
  };

  const [formData, setFormData] = useState({
    id: 0,
    region: '',
    country: '',
    itemType: '',
    salesChannel: '',
    orderPriority: '',
    orderDate: '',
    orderID: 0,
    shipDate: '',
    unitsSold: 0,
    unitPrice: 0,
    unitCost: 0,
    totalRevenue: 0,
    totalCost: 0,
    totalProfit: 0
  });

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const [showCE, setShowCE] = useState(false);
  const [modalCEData, setModalCEData] = useState({ status: '' });

  const handleCEShow = (data, status) => {
    setModalCEData({ status: status });
    if (status === "edit") {
      data.orderDate =  moment(data.orderDate).format('YYYY-MM-DD');
      data.shipDate =  moment(data.shipDate).format('YYYY-MM-DD');
      setFormData(data);
    }
    setShowCE(true);
  };

  const handleCEClose = () => {
    setShowCE(false);
    setFormData({
      id: 0,
      region: '',
      country: '',
      itemType: '',
      salesChannel: '',
      orderPriority: '',
      orderDate: '',
      orderID: 0,
      shipDate: '',
      unitsSold: 0,
      unitPrice: 0,
      unitCost: 0,
      totalRevenue: 0,
      totalCost: 0,
      totalProfit: 0
    });
  };

  const handleCreateSubmit = () => {
    createRecord(formData);
    setShowCE(false);
    setFormData({
      id: 0,
      region: '',
      country: '',
      itemType: '',
      salesChannel: '',
      orderPriority: '',
      orderDate: '',
      orderID: 0,
      shipDate: '',
      unitsSold: 0,
      unitPrice: 0,
      unitCost: 0,
      totalRevenue: 0,
      totalCost: 0,
      totalProfit: 0
    });
  };

  const handleEditSubmit = () => {
    updateRecord(formData.id, formData);
    setShowCE(false);
    setFormData({
      id: 0,
      region: '',
      country: '',
      itemType: '',
      salesChannel: '',
      orderPriority: '',
      orderDate: '',
      orderID: 0,
      shipDate: '',
      unitsSold: 0,
      unitPrice: 0,
      unitCost: 0,
      totalRevenue: 0,
      totalCost: 0,
      totalProfit: 0
    });
  };

  return (
    <Container fluid>
      <Modal show = {show} onHide = {handleClose}>
        <Modal.Header>
          <Modal.Title>Delete Record</Modal.Title>
        </Modal.Header>
        <Modal.Body>{modalData.body}</Modal.Body>
        <Modal.Footer>
          <Button variant = "secondary" onClick = {handleClose}>
            Close
          </Button>
          <Button variant = "primary" onClick = {handleDeleteSubmit}>
            Delete
          </Button>
        </Modal.Footer>
      </Modal>
      <Modal show = {showCE} onHide = {handleCEClose}>
        <Modal.Header>
          <Modal.Title>{modalCEData.status === "edit" ? "Edit Record" : "Create Record" }</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            <Form.Group controlId = "formRegion">
              <Form.Label>Region:</Form.Label>
              <Form.Control type = "text" name = "region" value = {formData.region} onChange = {handleChange} />
            </Form.Group>

            <Form.Group controlId = "formCountry">
              <Form.Label>Country:</Form.Label>
              <Form.Control type = "text" name = "country" value = {formData.country} onChange = {handleChange} />
            </Form.Group>

            <Form.Group controlId = "formItemType">
              <Form.Label>Item Type:</Form.Label>
              <Form.Control type = "text" name = "itemType" value = {formData.itemType} onChange = {handleChange} />
            </Form.Group>

            <Form.Group controlId = "formSalesChannel">
              <Form.Label>Sales Channel:</Form.Label>
              <Form.Control type = "text" name = "salesChannel" value = {formData.salesChannel} onChange = {handleChange} />
            </Form.Group>

            <Form.Group controlId = "formOrderPriority">
              <Form.Label>Order Priority:</Form.Label>
              <Form.Control type = "text" name = "orderPriority" value = {formData.orderPriority} onChange = {handleChange} />
            </Form.Group>

            <Form.Group controlId = "formOrderDate">
              <Form.Label>Order Date:</Form.Label>
              <Form.Control type = "date" name = "orderDate" value = {formData.orderDate} onChange = {handleChange} />
            </Form.Group>

            <Form.Group controlId = "formOrderID">
              <Form.Label>Order ID:</Form.Label>
              <Form.Control type="number" name = "orderID" value = {formData.orderID} onChange = {handleChange} />
            </Form.Group>

            <Form.Group controlId = "formShipDate">
              <Form.Label>Ship Date:</Form.Label>
              <Form.Control type = "date" name = "shipDate" value = {formData.shipDate} onChange = {handleChange} />
            </Form.Group>

            <Form.Group controlId = "formUnitsSold">
              <Form.Label>Units Sold:</Form.Label>
              <Form.Control type = "number" name = "unitsSold" value = {formData.unitsSold} onChange = {handleChange} />
            </Form.Group>

            <Form.Group controlId = "formUnitPrice">
              <Form.Label>Unit Price:</Form.Label>
              <Form.Control type = "number" name = "unitPrice" value = {formData.unitPrice} onChange = {handleChange} />
            </Form.Group>

            <Form.Group controlId = "formUnitCost">
              <Form.Label>Unit Cost:</Form.Label>
              <Form.Control type = "number" name = "unitCost" value = {formData.unitCost} onChange = {handleChange} />
            </Form.Group>

            <Form.Group controlId = "formTotalRevenue">
              <Form.Label>Total Revenue:</Form.Label>
              <Form.Control type = "number" name = "totalRevenue" value = {formData.totalRevenue} onChange = {handleChange} />
            </Form.Group>

            <Form.Group controlId = "formTotalCost">
              <Form.Label>Total Cost:</Form.Label>
              <Form.Control type = "number" name = "totalCost" value = {formData.totalCost} onChange = {handleChange} />
            </Form.Group>

            <Form.Group controlId = "formTotalProfit">
              <Form.Label>Total Profit:</Form.Label>
              <Form.Control type = "number" name = "totalProfit" value = {formData.totalProfit} onChange = {handleChange} />
            </Form.Group>
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant = "secondary" onClick = {handleCEClose}>
            Close
          </Button>
          {modalCEData.status === 'edit' ? (
              <Button variant = "primary" onClick = {handleEditSubmit}>
                Edit
              </Button>
            ) : (
              <Button variant = "primary" onClick = {handleCreateSubmit}>
                Create
              </Button>
            )
          }
        </Modal.Footer>
      </Modal>
      <Row>
        <Col>
          <Card>
            <Card.Header>
              <Form.Control
                type = "text"
                placeholder = "search for region"
                value = {region}
                onChange = {e => setRegion(e.target.value)}
              />
              <Form.Control
                type = "text"
                placeholder = "search for country"
                value = {country}
                onChange = {e => setCountry(e.target.value)}
              />
              <Button variant = "primary" onClick = {searchButton}>Search</Button>
              <Button variant = "primary" onClick = {resetButton}>Reset</Button>
              <Button variant = "success" onClick = {() => handleCEShow(null, "create")}>Create</Button>
            </Card.Header>
            <Card.Body>
              <DataTable
                columns = {columns}
                data = {recordList}
                progressPending = {isLoading}
                pagination
                paginationServer
                paginationTotalRows = {totalRows}
                onChangeRowsPerPage = {handlePerRowsChange}
                onChangePage = {handlePageChange}
                onSort = {handleSort}
                sortServer
                striped
                defaultSortFieldId = {1}
                defaultSortAsc = {true}
              />
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  )
}

export default App
