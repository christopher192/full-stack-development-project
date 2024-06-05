import React, { useEffect, useState } from 'react';
import { Container, Row, Col, Card, CardBody } from "reactstrap";
import axios from "axios";
import BreadCrumb from "../../Components/Common/BreadCrumb";
import Render from './Render';
import { useNavigate } from "react-router-dom";
import $ from 'jquery';
import DataTable from 'react-data-table-component';

const Home = () => {
    document.title = "Home | Sophic Dashboard Builder";
    const [data, setData] = useState({});
    const userProfile = sessionStorage.getItem("authUser") || null;
    const navigate = useNavigate();

    useEffect(() => {
        if (userProfile === null) {
            navigate("/login");
        }

        if (Object.keys(data).length === 0) {
            axios.get("/DashboardBuilderDatas/GetDashboardBuilderData?id=1").then(response => {
                setData(JSON.parse(response.data));
            });
        }
    }, [data, userProfile, navigate]);
    
    return (
        <React.Fragment>
            <div className="page-content">
                <Container fluid={true}>
                    <BreadCrumb title="Home" pageTitle="Dashboard" />
                    { Object.keys(data).length > 0 ? <Render data={data} /> : 
                        <Row>
                            <Col xl={12}>
                                <Card>
                                    <CardBody>
                                        <div className="d-flex justify-content-center align-items-center" style={{height: "450px"}}>';
                                            <h1 className="display-4">No Display Yet</h1>
                                        </div>
                                    </CardBody>
                                </Card>
                            </Col>
                        </Row>
                    }
                </Container>
            </div>
        </React.Fragment >
    );
};

export default Home;