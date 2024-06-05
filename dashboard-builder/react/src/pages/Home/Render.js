import React, { useEffect, useState } from 'react';
import { Card, CardBody, Col, Row } from 'reactstrap';
import parse from 'html-react-parser';

const Render = ({ data }) => {
    let components = data;
    const [html, setHTML] = useState('');
    const [script, setScript] = useState('');
    const [scriptLoaded, setScriptLoaded] = useState(false);
    const [createInterval, setCreateInterval] = useState(null);

    function randomIntFromInterval(min, max) {
        return Math.floor(Math.random() * (max - min + 1) + min);
    }

    useEffect(() => {
        setScriptLoaded(true);
        let interval;

        if (html === '') {
            let sample = '';
            let script = '';
            let widgetBoxIdLists = [];
        
            
            sample += '<div id="dashboard">';
            components.forEach(element => {
                if (element['type'] === 'containerRow') {
                    sample += '<div class="row">';
                        if (element.components !== undefined) {
                            if (element.components.length > 0) {
                                element.components.forEach((x) => {
                                    if (x['type'] === 'containerColumn') {
                                        sample += `<div class="${x['classes'].join(" ")}">`;
                                            if (x.components !== undefined) {
                                                if (x.components.length > 0 && x.components.length < 2) {
                                                    if (x.components[0]['type'] === 'barChart' || x.components[0]['type'] === 'pieChart' || x.components[0]['type'] === 'lineChart' || x.components[0]['type'] === 'gaugeChart') {
                                                        sample += `<canvas id="${x.components[0]['attributes']['id']}">`;
                                                        sample += '</canvas>';
                                                        script += "(function(){" + x.components[0]['script'] + "})();";
                                                    }
                                                    else if (x.components[0]['type'] === 'dataTable') {
                                                        sample += x.components[0].content;
                                                        script += "(function(){" + x.components[0]['script'] + "})();";
                                                    }
                                                    else if (x.components[0]['type'] === 'widgetBox') {
                                                        sample += x.components[0].content;
                                                        script += "(function(){" + x.components[0]['script'] + "})();";
                                                        widgetBoxIdLists.push(x.components[0].attributes.uniqueId);
                                                    }
                                                }
                                            }
                                        sample += '</div>';
                                    }
                                });
                            }
                        }
                        else {
                            sample += '<div class="col-12">';
                            sample += '<div class="d-flex justify-content-center align-items-center" style="height: 450px;">';
                                sample += '<h1 class="display-4">No Display Yet</h1>';
                            sample += '</div>';
                            sample += '</div>';
                        }
                    sample += '</div>';
                }
            });
            
            sample += '</div>';
            setHTML(sample);

            if (widgetBoxIdLists.length > 0) {
                // console.log(widgetBoxIdLists.join(', '));
                // script += "widgetBoxList = [" + widgetBoxIdLists.join(', ') +  "];";
                // script += "setIntervalF();";

                setCreateInterval(setInterval(() => {
                    widgetBoxIdLists.forEach(x => {
                        let randomNbr = randomIntFromInterval(100, 500);
                        let el = document.getElementById(x);
                        if (el) {
                            el.innerHTML = randomNbr + " K";
                        }
                    });
                }, 3000));
            }

            setScript(script);
        }

        if (document.getElementById("dashboard") !== null && document.getElementById("dashboard") !== '') {
            const getChartJSScript = document.getElementById('test');

            if (!getChartJSScript) {
                const s = document.createElement('script');
                s.id = 'test';
                s.type = 'text/javascript';
                s.async = true;
                s.innerHTML = script;
                document.body.append(s);
            }
            else {
                getChartJSScript.remove();

                const s = document.createElement('script');
                s.id = 'test';
                s.type = 'text/javascript';
                s.async = true;
                s.innerHTML = script;
                document.body.append(s);
            }
        }

        return () => {
            console.log(scriptLoaded);

            if (createInterval !== null) {
                clearInterval(createInterval);
            }

            scriptLoaded && document.body.removeChild(document.getElementById("test"));
        };

    }, [components, html, script, scriptLoaded, createInterval]);

    return (
        <React.Fragment>
            <Row>
                <Col xl={12}>
                    <Card>
                        <CardBody>
                        {
                            parse(html)
                        }
                        </CardBody>
                    </Card>
                </Col>
            </Row>
        </React.Fragment>
    );
};

export default Render;