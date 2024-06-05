import React, { useState, useEffect, useRef } from 'react';
import { Card, CardBody, CardHeader, Col, Row } from 'reactstrap';
import { Container } from 'reactstrap';
import { useQuery, gql } from '@apollo/client';
import { Target } from './Target';

const Dashboard = () => {
    document.title="Dashboard | Digital Counter Dashboard";
    const [timeText, setTimeText] = useState(null);
    const [machineTargetData, setmachineTarget] = useState(null);
    const timeInterval = useRef(null);
    const sams1Interval = useRef(null); 
    const sams2Interval = useRef(null);
    const sams3Interval = useRef(null);
    const sams4Interval = useRef(null);
    const sams5Interval = useRef(null);
    const sams6Interval = useRef(null);
    const sams7Interval = useRef(null);
    const sams8Interval = useRef(null);
    const sams9Interval = useRef(null);

    const GET_TARGET_MACHINE = gql`
        query GetMachineTarget($mode: String!) {
            machineTarget(mode: $mode) {
                id,
                dimension,
                machine_name,
                status,
                target_morning,
                target_afternoon,
                target_night,
                sum_total_input,
                sum_good_operator_1,
                sum_good_operator_2,
                sum_total_good,
                sum_total_good_cutting
            }
        }
    `;

    const { loading, error, data, refetch } = useQuery(GET_TARGET_MACHINE, {
        fetchPolicy: "no-cache",
        pollInterval: 300000, // Set the polling interval to 5 minutes (300,000 milliseconds)
        variables: {
            mode: timeText
        },
        skip: !timeText,
        onCompleted: (result) => {
            console.log("hello from usequery");
        }
    });

    useEffect(() => {
        timeInterval.current = setInterval(() => { 
            let currentNewTime = new Date();
            let currentHour = currentNewTime.getHours();
            document.getElementById("currentTime").innerHTML = currentNewTime.toLocaleString();

            if (currentHour >= 7 && currentHour < 15) {
                if (timeText === null) {
                    setTimeText('Morning Shift');
                } else if (timeText !== 'Morning Shift') {
                    setTimeText('Morning Shift');
                }
            } else if (currentHour >= 15 && currentHour < 23) {
                if (timeText === null) {
                    setTimeText('Afternoon Shift');
                } else if (timeText !== 'Afternoon Shift') {
                    setTimeText('Afternoon Shift');
                }
            } else {
                if (timeText === null) {
                    setTimeText('Night Shift');
                } else if (timeText !== 'Night Shift') {
                    setTimeText('Night Shift');
                }
            }
        }, 1000);

        return () => {
            clearInterval(timeInterval.current);
        };
    }, [timeText]);

    useEffect(() => { 
        if (loading === false && data && timeText) {
            console.log("from second: ", data);
            let machineData = [];
            let intervaMachinelList = {};
            const seconds = 28800;
            
            data.machineTarget.map(x => {
                let target = 0;

                if (timeText === 'Morning Shift') {
                    let machineMorningTargetSecondInterval = Math.round(seconds/ x['target_morning']);
    
                    let currentDateTime = new Date();
                    let currentHours = currentDateTime.getHours();
                    let currentMinutes = currentDateTime.getMinutes();
                    let currentSeconds = currentDateTime.getSeconds();

                    // Minus start hours to get working hours
                    let workingHour = ((currentHours - 7) * 60 * 60) + (currentMinutes * 60) + currentSeconds;

                    target = Math.round(workingHour/ machineMorningTargetSecondInterval);
                    intervaMachinelList[x.machine_name] = machineMorningTargetSecondInterval;
                } else if (timeText === 'Afternoon Shift') {
                    let machineAfternoonTargetSecondInterval = Math.round(seconds/ x['target_afternoon']);
                    
                    let currentDateTime = new Date();
                    let currentHours = currentDateTime.getHours();
                    let currentMinutes = currentDateTime.getMinutes();
                    let currentSeconds = currentDateTime.getSeconds();

                    // Minus start hours to get working hours
                    let workingHour = ((currentHours - 15) * 60 * 60) + (currentMinutes * 60) + currentSeconds;

                    target = Math.round(workingHour/ machineAfternoonTargetSecondInterval);
                    intervaMachinelList[x.machine_name] = machineAfternoonTargetSecondInterval;
                } else if (timeText === 'Night Shift') {
                    let machineAfternoonTargetSecondInterval = Math.round(seconds/ x['target_night']);
                    
                    let currentDateTime = new Date();
                    let currentHours = currentDateTime.getHours();
                    let currentMinutes = currentDateTime.getMinutes();
                    let currentSeconds = currentDateTime.getSeconds();

                    currentHours = 2;
                    let workingHour = 0; 
                    
                    // If 23 hrs then set it become 0 else + 1     
                    if (currentHours === 23) {
                        workingHour = (currentMinutes * 60) + currentSeconds;
                    } else if (currentHours >= 1 && currentHours < 7) {
                        workingHour = ((currentHours + 1) * 60 * 60) + (currentMinutes * 60) + currentSeconds;               
                    }

                    target = Math.round(workingHour/ machineAfternoonTargetSecondInterval);
                    intervaMachinelList[x.machine_name] = machineAfternoonTargetSecondInterval;
                }
                
                let delta = {
                    'value': null,
                    'color': ''
                };

                if (target !== 0 && x.sum_total_input !== null) {
                    delta.value = target - x.sum_total_input;
                    if (delta.value >= 20) {
                        delta.color = 'red';
                    } else if (delta.value > 5 && delta.value < 20){
                        delta.color = 'yellow';
                    } else if (delta.value <= 5) {
                        delta.color = 'green';           
                    }
                }

                let overall_yield = null;
                let machine_yield = null;
                
                if (x.sum_good_operator_1 !== null && x.sum_good_operator_2 !== null && x.sum_total_input !== null) {
                    overall_yield = Math.round((Number(x.sum_good_operator_1) + Number(x.sum_good_operator_2))/ Number(x.sum_total_input));
                }

                if (x.sum_total_good !== null && x.sum_total_good_cutting !== null){
                    machine_yield = Math.round(Number(x.sum_total_good)/ Number(x.sum_total_good_cutting));
                }

                machineData.push({'id': x.id, 'dimension': x.dimension, 'machine_name': x.machine_name, 
                    'status': x.status, 'target': target, 'sum_total_input': x.sum_total_input, 'delta': delta, 'overall_yield': overall_yield, 'machine_yield': machine_yield });

                return x;
            });
            
            setmachineTarget(machineData);
            console.log(machineData);
            Object.entries(intervaMachinelList).forEach(([key, value]) => {
                if (key === "SASM 1") {
                    sams1Interval.current = setInterval(() => {
                        let target = 0;
                        if (document.getElementById("SASM 1").innerHTML) {
                            target = Number(document.getElementById("SASM 1").innerHTML) + 1;
                            document.getElementById("SASM 1").innerHTML = target;
                        }
                        if (document.getElementById("delta_SASM 1")) {
                            if (document.getElementById("delta_SASM 1").getAttribute('data-value') !== null) {
                                let element = document.getElementById("delta_SASM 1");
                                let sumTotalInput = document.getElementById("delta_SASM 1").getAttribute('data-value');
                                let delta = target - Number(sumTotalInput);

                                element.innerHTML = delta;

                                if (delta >= 20) {
                                    element.style.color = 'red';
                                } else if (delta > 5 && delta < 20){
                                    element.style.color = 'yellow';
                                } else if (delta <= 5) {
                                    element.style.color = 'green';          
                                }
                            }
                            else {
                                let element = document.getElementById("delta_SASM 1");
                                element.innerHTML = 'NA';
                                element.style.color = '';
                            }
                        }
                    }, value * 1000);
                } else if (key === "SASM 2") {
                    sams2Interval.current = setInterval(() => {
                        let target = 0;
                        if (document.getElementById("SASM 2").innerHTML) {
                            target = Number(document.getElementById("SASM 2").innerHTML) + 1;
                            document.getElementById("SASM 2").innerHTML = target;
                        }
                        if (document.getElementById("delta_SASM 2")) {
                            if (document.getElementById("delta_SASM 2").getAttribute('data-value') !== null) {
                                let element = document.getElementById("delta_SASM 2");
                                let sumTotalInput = document.getElementById("delta_SASM 2").getAttribute('data-value');
                                let delta = target - Number(sumTotalInput);

                                element.innerHTML = delta;

                                if (delta >= 20) {
                                    element.style.color = 'red';
                                } else if (delta > 5 && delta < 20){
                                    element.style.color = 'yellow';
                                } else if (delta <= 5) {
                                    element.style.color = 'green';          
                                }
                            }
                            else {
                                let element = document.getElementById("delta_SASM 2");
                                element.innerHTML = 'NA';
                                element.style.color = '';
                            }
                        }
                    }, value * 1000);
                } else if (key === "SASM 3") {
                    sams3Interval.current = setInterval(() => {
                        let target = 0;
                        if (document.getElementById("SASM 3").innerHTML) {
                            target = Number(document.getElementById("SASM 3").innerHTML) + 1;
                            document.getElementById("SASM 3").innerHTML = target;
                        }
                        if (document.getElementById("delta_SASM 3")) {
                            if (document.getElementById("delta_SASM 3").getAttribute('data-value') !== null) {
                                let element = document.getElementById("delta_SASM 3");
                                let sumTotalInput = document.getElementById("delta_SASM 3").getAttribute('data-value');
                                let delta = target - Number(sumTotalInput);

                                element.innerHTML = delta;

                                if (delta >= 20) {
                                    element.style.color = 'red';
                                } else if (delta > 5 && delta < 20){
                                    element.style.color = 'yellow';
                                } else if (delta <= 5) {
                                    element.style.color = 'green';          
                                }
                            }
                            else {
                                let element = document.getElementById("delta_SASM 3");
                                element.innerHTML = 'NA';
                                element.style.color = '';
                            }
                        }
                    }, value * 1000);
                } else if (key === "SASM 4") {
                    sams4Interval.current = setInterval(() => {
                        let target = 0;
                        if (document.getElementById("SASM 4").innerHTML) {
                            target = Number(document.getElementById("SASM 4").innerHTML) + 1;
                            document.getElementById("SASM 4").innerHTML = target;
                        }
                        if (document.getElementById("delta_SASM 4")) {
                            if (document.getElementById("delta_SASM 4").getAttribute('data-value') !== null) {
                                let element = document.getElementById("delta_SASM 4");
                                let sumTotalInput = document.getElementById("delta_SASM 4").getAttribute('data-value');
                                let delta = target - Number(sumTotalInput);

                                element.innerHTML = delta;

                                if (delta >= 20) {
                                    element.style.color = 'red';
                                } else if (delta > 5 && delta < 20){
                                    element.style.color = 'yellow';
                                } else if (delta <= 5) {
                                    element.style.color = 'green';          
                                }
                            }
                            else {
                                let element = document.getElementById("delta_SASM 4");
                                element.innerHTML = 'NA';
                                element.style.color = '';
                            }
                        }
                    }, value * 1000);
                } else if (key === "SASM 5") {
                    sams5Interval.current = setInterval(() => {
                        let target = 0;
                        if (document.getElementById("SASM 5").innerHTML) {
                            target = Number(document.getElementById("SASM 5").innerHTML) + 1;
                            document.getElementById("SASM 5").innerHTML = target;
                        }
                        if (document.getElementById("delta_SASM 5")) {
                            if (document.getElementById("delta_SASM 5").getAttribute('data-value') !== null) {
                                let element = document.getElementById("delta_SASM 5");
                                let sumTotalInput = document.getElementById("delta_SASM 5").getAttribute('data-value');
                                let delta = target - Number(sumTotalInput);

                                element.innerHTML = delta;

                                if (delta >= 20) {
                                    element.style.color = 'red';
                                } else if (delta > 5 && delta < 20){
                                    element.style.color = 'yellow';
                                } else if (delta <= 5) {
                                    element.style.color = 'green';          
                                }
                            }
                            else {
                                let element = document.getElementById("delta_SASM 5");
                                element.innerHTML = 'NA';
                                element.style.color = '';
                            }
                        }
                    }, value * 1000);
                } else if (key === "SASM 6") {
                    sams6Interval.current = setInterval(() => {
                        let target = 0;
                        if (document.getElementById("SASM 6").innerHTML) {
                            target = Number(document.getElementById("SASM 6").innerHTML) + 1;
                            document.getElementById("SASM 6").innerHTML = target;
                        }
                        if (document.getElementById("delta_SASM 6")) {
                            if (document.getElementById("delta_SASM 6").getAttribute('data-value') !== null) {
                                let element = document.getElementById("delta_SASM 6");
                                let sumTotalInput = document.getElementById("delta_SASM 6").getAttribute('data-value');
                                let delta = target - Number(sumTotalInput);

                                element.innerHTML = delta;

                                if (delta >= 20) {
                                    element.style.color = 'red';
                                } else if (delta > 5 && delta < 20){
                                    element.style.color = 'yellow';
                                } else if (delta <= 5) {
                                    element.style.color = 'green';          
                                }
                            }
                            else {
                                let element = document.getElementById("delta_SASM 6");
                                element.innerHTML = 'NA';
                                element.style.color = '';
                            }
                        }
                    }, value * 1000);
                } else if (key === "SASM 7") {
                    sams7Interval.current = setInterval(() => {
                        let target = 0;
                        if (document.getElementById("SASM 7").innerHTML) {
                            target = Number(document.getElementById("SASM 7").innerHTML) + 1;
                            document.getElementById("SASM 7").innerHTML = target;
                        }
                        if (document.getElementById("delta_SASM 7")) {
                            if (document.getElementById("delta_SASM 7").getAttribute('data-value') !== null) {
                                let element = document.getElementById("delta_SASM 7");
                                let sumTotalInput = document.getElementById("delta_SASM 7").getAttribute('data-value');
                                let delta = target - Number(sumTotalInput);

                                element.innerHTML = delta;

                                if (delta >= 20) {
                                    element.style.color = 'red';
                                } else if (delta > 5 && delta < 20){
                                    element.style.color = 'yellow';
                                } else if (delta <= 5) {
                                    element.style.color = 'green';          
                                }
                            }
                            else {
                                let element = document.getElementById("delta_SASM 7");
                                element.innerHTML = 'NA';
                                element.style.color = '';
                            }
                        }
                    }, value * 1000);
                } else if (key === "SASM 8") {
                    sams8Interval.current = setInterval(() => {
                        let target = 0;
                        if (document.getElementById("SASM 8").innerHTML) {
                            target = Number(document.getElementById("SASM 8").innerHTML) + 1;
                            document.getElementById("SASM 8").innerHTML = target;
                        }
                        if (document.getElementById("delta_SASM 8")) {
                            if (document.getElementById("delta_SASM 8").getAttribute('data-value') !== null) {
                                let element = document.getElementById("delta_SASM 8");
                                let sumTotalInput = document.getElementById("delta_SASM 8").getAttribute('data-value');
                                let delta = target - Number(sumTotalInput);

                                element.innerHTML = delta;

                                if (delta >= 20) {
                                    element.style.color = 'red';
                                } else if (delta > 5 && delta < 20){
                                    element.style.color = 'yellow';
                                } else if (delta <= 5) {
                                    element.style.color = 'green';          
                                }
                            }
                            else {
                                let element = document.getElementById("delta_SASM 8");
                                element.innerHTML = 'NA';
                                element.style.color = '';
                            }
                        }
                    }, value * 1000);
                } else if (key === "SASM 9") {
                    sams9Interval.current = setInterval(() => {
                        let target = 0;
                        if (document.getElementById("SASM 9").innerHTML) {
                            target = Number(document.getElementById("SASM 9").innerHTML) + 1;
                            document.getElementById("SASM 9").innerHTML = target;
                        }
                        if (document.getElementById("delta_SASM 9")) {
                            if (document.getElementById("delta_SASM 9").getAttribute('data-value') !== null) {
                                let element = document.getElementById("delta_SASM 9");
                                let sumTotalInput = document.getElementById("delta_SASM 9").getAttribute('data-value');
                                let delta = target - Number(sumTotalInput);

                                element.innerHTML = delta;

                                if (delta >= 20) {
                                    element.style.color = 'red';
                                } else if (delta > 5 && delta < 20){
                                    element.style.color = 'yellow';
                                } else if (delta <= 5) {
                                    element.style.color = 'green';          
                                }
                            }
                            else {
                                let element = document.getElementById("delta_SASM 9");
                                element.innerHTML = 'NA';
                                element.style.color = '';
                            }
                        }
                    }, value * 1000);
                }
            });
        }

        return () => {
            clearInterval(sams1Interval.current);
            clearInterval(sams2Interval.current);
            clearInterval(sams3Interval.current);
            clearInterval(sams4Interval.current);
            clearInterval(sams5Interval.current);
            clearInterval(sams6Interval.current);
            clearInterval(sams7Interval.current);
            clearInterval(sams8Interval.current);
            clearInterval(sams9Interval.current);
        };
    }, [data, timeText, loading]);

    function handleSubmit(e) {
        e.preventDefault();
        refetch({
            mode: timeText
        });
    }

    function handleMorningShiftSubmit(e) {
        e.preventDefault();
        setTimeText("Morning Shift");
    }

    function handleAfternoonShiftSubmit(e) {
        e.preventDefault();
        setTimeText("Afternoon Shift");
    }

    function handleNightShiftSubmit(e) {
        e.preventDefault();
        setTimeText("Night Shift");
    }

    return (
        <React.Fragment>
            <div className="page-content">
                <Container fluid>
                    <div className="d-flex justify-content-center vh-20">
                        <Card className="w-50" style={{ boxShadow: '0 2px 4px rgba(0, 0, 0, 0.2)' }}>
                            <Row>
                                <Col className="d-flex justify-content-center">
                                    <CardBody>
                                        <h3 id="currentTime">{new Date().toLocaleString()}</h3> 
                                    </CardBody>
                                </Col>
                                <Col className="d-flex justify-content-center" >
                                    <CardBody style={{ background: 'linear-gradient(45deg,#4099ff,#73b4ff)' }}>
                                        <h3 style={{ color: '#ffffff' }}>{timeText}</h3>
                                    </CardBody>
                                </Col>
                            </Row>
                        </Card>
                    </div>
                    <div>
                        <button className='btn btn-primary' onClick={handleSubmit}>Refetch</button>
                        <button className='btn btn-primary' onClick={handleMorningShiftSubmit}>Set Morning Shift</button>
                        <button className='btn btn-primary' onClick={handleAfternoonShiftSubmit}>Set Afternoon Shift</button>
                        <button className='btn btn-primary' onClick={handleNightShiftSubmit}>Set Night Shift</button>
                    </div>
                    <div className="d-flex justify-content-between">
                        {machineTargetData?.map((x) => (
                            <Card style={{ boxShadow: '0 2px 4px rgba(0, 0, 0, 0.2)' }} key = {x.id.toString()}>
                                <CardHeader style={{ background: 'linear-gradient(45deg,#FFB64D,#ffcb80)', color: '#ffffff' }} className="text-center">
                                    <h1>{x.machine_name}</h1>
                                </CardHeader>
                                <CardBody className="text-center">
                                    <h2>{x.dimension}</h2>
                                    <hr />
                                    <h5>Target</h5>
                                    <h1 id={x.machine_name} style={{ fontSize: '6em' }}>{x.target}</h1>            
                                    <hr />
                                    <h5>Output</h5>
                                    <h1 style={{ fontSize: '6em' }}>{x.sum_total_input ? x.sum_total_input : 'NA'}</h1>
                                    <hr />
                                    <h5>Delta</h5>
                                    {x.delta.value === null ? (<h1 id={"delta_" + x.machine_name} style={{ fontSize: '6em', color: x.delta.color }} data-value={x.sum_total_input}>NA</h1>) : (<h1 id={"delta_" + x.machine_name} style={{ fontSize: '6em', color: x.delta.color }} data-value={x.sum_total_input}>{x.delta.value}</h1>)}
                                    <hr />
                                    <h5>Overall Yield</h5>
                                    {x.overall_yield === null ? (<h1>NA</h1>) : (<h1>{x.overall_yield}</h1>)}
                                    <hr />
                                    <h5>Machine Yield</h5>
                                    {x.machine_yield === null ? (<h1>NA</h1>) : (<h1>{x.machine_yield}</h1>)}
                                </CardBody>
                            </Card>
                        ))}
                    </div>
                </Container>
            </div>
        </React.Fragment>
    );
};

export default Dashboard;

