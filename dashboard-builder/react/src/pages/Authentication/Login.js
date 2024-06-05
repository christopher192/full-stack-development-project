import React, { useEffect, useState } from 'react';
import { Card, CardBody, Col, Container, Input, Label, Row, Button, Form, FormFeedback, Alert, Spinner } from 'reactstrap';
import ParticlesAuth from "../AuthenticationInner/ParticlesAuth";

// redux
import { useSelector, useDispatch } from "react-redux";

// Formik validation
import * as Yup from "yup";
import { useFormik } from "formik";

// actions
import { loginUser, resetLoginFlag } from "../../store/actions";

import withRouter from "../../Components/Common/withRouter";
import logo from "../../assets/images/Sophic Logo2.png";

const Login = (props) => {
    const dispatch = useDispatch();
    const { user, errorMsg, loading, error } = useSelector(state => ({
        user: state.Account.user,
        errorMsg: state.Login.errorMsg,
        loading: state.Login.loading,
        error: state.Login.error,
    }));

    const [userLogin, setUserLogin] = useState([]);
    const [passwordShow, setPasswordShow] = useState(false);

    useEffect(() => {
        if (user) {
            setUserLogin({
                userName: user.user.userName,
                password: user.user.confirm_password
            });
        }
    }, [user]);

    const validation = useFormik({
        // enableReinitialize : use this flag when initial values needs to be changed
        enableReinitialize: true,

        initialValues: {
            userName: userLogin.userName || "admin" || '',
            password: userLogin.password || "admin" || '',
        },
        validationSchema: Yup.object({
            userName: Yup.string().required("Please Enter Your User Name"),
            password: Yup.string().required("Please Enter Your Password"),
        }),
        onSubmit: (values) => {
            dispatch(loginUser(values, props.router.navigate));
        }
    });

    useEffect(() => {
        if (error) {
            setTimeout(() => {
                dispatch(resetLoginFlag());
            }, 3000);
        }
    }, [dispatch, error]);

    document.title = "Login | Sophic Dashboard Builder";
    return (
        <React.Fragment>
            <ParticlesAuth>
                <div className="auth-page-content overflow-hidden pt-lg-5">
                    <Container>
                        <Row className="justify-content-center">
                            <Col md={8} lg={7} xl={7}>
                                <Card className="mt-4">
                                    <CardBody className="p-4">
                                        <div className="d-flex justify-content-between">
                                            <div>
                                                <h5 className="text-primary">Welcome Back !</h5>
                                                <p className="text-muted">Sign in to dashboard builder.</p>
                                            </div>
                                            <img src={logo} alt="" height="60" />
                                        </div>
                                        {errorMsg && errorMsg ? (<Alert color="danger"> {errorMsg} </Alert>) : null}
                                        <div className="p-2 mt-4">
                                            <Form
                                                onSubmit={(e) => {
                                                    e.preventDefault();
                                                    validation.handleSubmit();
                                                    return false;
                                                }}
                                                action="#">

                                                <div className="mb-3">
                                                    <Label htmlFor="userName" className="form-label">User Name</Label>
                                                    <Input
                                                        name="userName"
                                                        className="form-control"
                                                        placeholder="Enter user name"
                                                        type="text"
                                                        onChange={validation.handleChange}
                                                        onBlur={validation.handleBlur}
                                                        value={validation.values.userName || ""}
                                                        invalid={
                                                            validation.touched.userName && validation.errors.userName ? true : false
                                                        }
                                                    />
                                                    {validation.touched.userName && validation.errors.userName ? (
                                                        <FormFeedback type="invalid">{validation.errors.userName}</FormFeedback>
                                                    ) : null}
                                                </div>

                                                <div className="mb-3">
                                                    <Label className="form-label" htmlFor="password-input">Password</Label>
                                                    <div className="position-relative auth-pass-inputgroup mb-3">
                                                        <Input
                                                            name="password"
                                                            value={validation.values.password || ""}
                                                            type={passwordShow ? "text" : "password"}
                                                            className="form-control pe-5"
                                                            placeholder="Enter Password"
                                                            onChange={validation.handleChange}
                                                            onBlur={validation.handleBlur}
                                                            invalid={
                                                                validation.touched.password && validation.errors.password ? true : false
                                                            }
                                                        />
                                                        {validation.touched.password && validation.errors.password ? (
                                                            <FormFeedback type="invalid">{validation.errors.password}</FormFeedback>
                                                        ) : null}
                                                        <button className="btn btn-link position-absolute end-0 top-0 text-decoration-none text-muted" type="button" id="password-addon" onClick={() => setPasswordShow(!passwordShow)}><i className="ri-eye-fill align-middle"></i></button>
                                                    </div>
                                                </div>

                                                <div className="mt-4">
                                                    <Button color="success" disabled={error ? null : loading ? true : false} className="btn btn-success w-100" type="submit">
                                                        {error ? null : loading ? <Spinner size="sm" className='me-2'> Loading... </Spinner> : null}
                                                        Sign In
                                                    </Button>
                                                </div>
                                            </Form>
                                        </div>
                                    </CardBody>
                                </Card>
                            </Col>
                        </Row>
                    </Container>
                </div>
            </ParticlesAuth>
        </React.Fragment>
    );
};

export default withRouter(Login);