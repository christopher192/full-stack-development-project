import { useState } from 'react'
import { Container, Row, Col } from 'react-bootstrap';
import DataTable from 'react-data-table-component';

function App() {
  const columns = [
    {
      name: 'Title',
      selector: row => row.title,
    },
    {
      name: 'Year',
      selector: row => row.year,
    },
  ];
  
  const data = [
      {
      id: 1,
      title: 'Beetlejuice',
      year: '1988',
    },
    {
      id: 2,
      title: 'Ghostbusters',
      year: '1984',
    },
  ]

  const [perPage, setPerPage] = useState(10);
	const [page, setPage] = useState(1);
	const [sortCol, setSortCol] = useState(1);
	const [sortDir, setSortDir] = useState("asc");

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

  return (
    <Container>
      <Row>
        <Col>
          <DataTable
            columns = {columns}
            data = {data}
            pagination
            paginationServer
            onChangeRowsPerPage={handlePerRowsChange}
            onChangePage={handlePageChange}
            onSort={handleSort}
            sortServer
            striped
            defaultSortFieldId={1}
            defaultSortAsc={true}
          />
        </Col>
      </Row>
    </Container>
  )
}

export default App
