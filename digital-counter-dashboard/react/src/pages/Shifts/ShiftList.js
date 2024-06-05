import React from 'react';
import { Card, CardHeader,CardBody, Container, Row, Col, Label, Input, Table } from 'reactstrap';
import UiContent from "../../Components/Common/UiContent";
import PreviewCardHeader from '../../Components/Common/PreviewCardHeader';

import { Link } from 'react-router-dom';


// import Components
import BreadCrumb from '../../Components/Common/BreadCrumb';

const ShiftList = () => {

    return (
        <React.Fragment>
            <UiContent />
            <div>
                <Container fluid>
                    <Row>
                        <Col xl={12}>
                            <Card>
                                <PreviewCardHeader title="Shift List" />
                                <CardBody>
                                    <div className="live-preview">
                                        <div className="table-responsive">
                                            <Table className="align-middle table-nowrap mb-0">
                                                <thead>
                                                    <tr>
                                                        <th scope="col">Date</th>
                                                        <th scope="col">Morning</th>
                                                        <th scope="col">Afternoon</th>
                                                        <th scope="col">Night</th>
                                                        <th scope="col">Action</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                  
                                                </tbody>
                                            </Table>
                                        </div>
                                    </div>

                                   
                                </CardBody>
                            </Card>
                        </Col>
                    </Row>
                </Container>
            </div>
        </React.Fragment>
    );
};

export default ShiftList;
