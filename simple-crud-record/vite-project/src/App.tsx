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

  return (
    <Container>
      <Row>
        <Col>
          <DataTable
            columns = {columns}
            data = {data}
          />
        </Col>
      </Row>
    </Container>
  )
}

export default App
