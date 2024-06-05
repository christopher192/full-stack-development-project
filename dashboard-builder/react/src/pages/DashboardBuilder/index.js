import React, { useEffect, useState } from 'react';
import ReactDOM from 'react-dom';
import grapesjs from 'grapesjs';
import './style.css';
import 'grapesjs/dist/css/grapes.min.css';
import { json, useNavigate } from "react-router-dom";
import axios from "axios";
import { v4 as uuidv4 } from 'uuid';
import _ from "lodash";
import DataTable from 'react-data-table-component';

const DashboardBuilder = () => {
    document.title = "Dashboard Builder | Sophic Dashboard Builder";
    
    let navigate = useNavigate();
    const [apiLists, setApiLists] = useState(null);
    const [projectData, setProjectData] = useState(null);
    const [componentsData, setComponentsData] = useState(null);
    const [widgetBoxColumnLists, setWidgetBoxColumnLists] = useState(null);
    const [widgetBoxDescriptionLists, setWidgetBoxDescriptionLists] = useState(null);
    const [widgetBoxIconLists, setwidgetBoxIconLists ]= useState({
        'Multi User' : '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-users text-info"><g><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"></path><circle cx="9" cy="7" r="4"></circle><path d="M23 21v-2a4 4 0 0 0-3-3.87"></path><path d="M16 3.13a4 4 0 0 1 0 7.75"></path></g></svg>',
        'Line' : '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-activity text-info"><g><polyline points="22 12 18 12 15 21 9 3 6 12 2 12"></polyline></g></svg>',
        'Time' : '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-clock text-info"><g><circle cx="12" cy="12" r="10"></circle><polyline points="12 6 12 12 16 14"></polyline></g></svg>',
        'Box' : '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-external-link text-info"><g><path d="M18 13v6a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h6"></path><polyline points="15 3 21 3 21 9"></polyline><line x1="10" y1="14" x2="21" y2="3"></line></g></svg>'
    });
    const [mssqlTable, setMssqlTable] = useState(null);

    function generateRandomId() { 
        let id = uuidv4();
        return id;
    }

    function generateDefaultScript(id, chartType, data) {
        let script = '';

        if (chartType === 'line') { 
            script = `new Chart(document.getElementById("${id}"), {
                type: '${chartType}',
                data: {
                labels: ["Africa", "Asia", "Europe", "Latin America", "North America"],
                datasets: [
                    {
                        label: "Population (millions)",
                        data: ${data !== undefined ? data : '[2478,5267,734,784,433]' },
                        fill: false,
                        borderColor: 'rgb(75, 192, 192)',
                        lineTension: 0,  
                    }
                ]
                },
                options: {
                    legend: { display: true },
                    title: {
                        display: true,
                        text: 'Predicted world population (millions) in 2050'
                    }
                }
            });`;
        }
        else {
            script = `new Chart(document.getElementById("${id}"), {
                type: '${chartType}',
                data: {
                labels: ["Africa", "Asia", "Europe", "Latin America", "North America"],
                datasets: [
                    {
                        label: "Population (millions)",
                        backgroundColor: ["#3e95cd", "#8e5ea2","#3cba9f","#e8c3b9","#c45850"],
                        data: ${data !== undefined ? data : '[2478,5267,734,784,433]' }
                    }
                ]
                },
                options: {
                    legend: { display: true },
                    title: {
                        display: true,
                        text: 'Predicted world population (millions) in 2050'
                    }
                }
            });`;
        }

        return script;
    }

    function generateScript(id, url, chartType) {
        let script = '';

        if (chartType === 'line') {
            script = `let cookie = JSON.parse(sessionStorage.getItem("authUser"));
                $.ajax({
                    type: "GET",
                    url: "${url}",
                    dataType: "json",
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader ("Authorization", "Bearer " + cookie.token);
                    },
                    success: function(resp) {
                        new Chart(document.getElementById("${id}"), {
                            type: '${chartType}',
                            data: {
                            labels: resp.labels,
                            datasets: [
                                {
                                    label: "Population (millions)",
                                    data: resp.data,
                                    fill: false,
                                    borderColor: 'rgb(75, 192, 192)',
                                    lineTension: 0, 
                                }
                            ]
                            },
                            options: {
                                legend: { display: true },
                                title: {
                                    display: true,
                                    text: 'Predicted world population (millions) in 2050'
                                }
                            }
                        });
                    },
                    error: function(xhr) {

                    },
                    complete: function() {

                    },
                });`;
        }
        else {
            script = `let cookie = JSON.parse(sessionStorage.getItem("authUser"));
                $.ajax({
                    type: "GET",
                    url: "${url}",
                    dataType: "json",
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader ("Authorization", "Bearer " + cookie.token);
                    },
                    success: function(resp) {
                        new Chart(document.getElementById("${id}"), {
                            type: '${chartType}',
                            data: {
                            labels: resp.labels,
                            datasets: [
                                {
                                    label: "Population (millions)",
                                    data: resp.data,
                                    backgroundColor: defaultChartColorsGrapesJS.slice(0, resp.data.length)
                                }
                            ]
                            },
                            options: {
                                legend: { display: true },
                                title: {
                                    display: true,
                                    text: 'Predicted world population (millions) in 2050'
                                }
                            }
                        });
                    },
                    error: function(xhr) {

                    },
                    complete: function() {

                    },
                });`;
        }

        return script;
    }

    function generateDataTableScript(status = 'default', uniqueId, 
        setting = { columns : { }, table: '', }) {
        let script = ``;

        if (status === 'default') {
            script = `
                $(document).ready(function () {
                    $('#${uniqueId}').DataTable({
                        responsive: true
                    });
                });         
            `;          
        }
        else {
            let baseURL = axios.defaults.baseURL;
            let url = `${baseURL}/DataTable/Load${setting.table}DataTable`;
            let columnss = [];

            Object.entries(setting.columns).map(([key, value]) => {
                if (value.displayable === true) {
                    columnss.push({ data: key, searchable: value.searchable && !value.disabled });
                }

                return '';
            });

            script = `
                $(document).ready(function () {
                    let cookie = JSON.parse(sessionStorage.getItem("authUser"));

                    let table = $('#${uniqueId}').DataTable({
                        stateSave: true,
                        processing: true,
                        serverSide: true,
                        paging: true,
                        responsive: true,
                        lengthMenu: [10, 25, 50, 100, 500, 1000, 2500, 5000],
                        dom: 'lrtip',
                        ajax: {
                            url: "${url}",
                            type: "POST",
                            "headers": {
                                "Authorization": "Bearer: " + cookie.token
                            },
                            contentType: "application/json",
                            dataType: "json",
                            data: function (d) {
                                return JSON.stringify(d);
                            }
                        },
                        columns: ${JSON.stringify(columnss)}
                    });

                    $('#${uniqueId}_search').on('click', function (e) {
                        e.preventDefault();
                        var params = {};
                        $('.datatable-${uniqueId}-input').each(function () {
                            var i = $(this).data('col-index');
                            if (params[i]) {
                                params[i] += '|' + $(this).val();
                            }
                            else {
                                params[i] = $(this).val();
                            }
                        });

                        $.each(params, function (i, val) {
                            table.column(i).search(val ? val : '', false, false);
                        });
                        table.table().draw();
                    });
        
                    $('#${uniqueId}_reset').on('click', function (e) {
                        e.preventDefault();
                        $('.datatable-${uniqueId}-input').each(function () {
                            $(this).val('');
                            table.column($(this).data('col-index')).search('', false, false);
                        });
                        table.table().draw();
                    });
                });     
            `;             
        }

        return script;
    }

    function generateDataTableHTML(status = 'default', uniqueId,
        setting = { columns : { }, table: '', }) {
        let html = ``;
        let columnsHTML = '';
        let searchBoxObj = { };

        Object.entries(setting.columns).map(([key, value]) => {
            if (value.displayable === true) {
                columnsHTML += '<th>' + _.startCase(key) + '</th>';

                if (value.searchable === true && value.disabled === false) {
                    searchBoxObj[key] = { index: value.index, fullName: value.fullName };
                }
            }

            return '';
        });

        if (status === 'default') {
            html = `<table id="${uniqueId}" class="table table-bordered dt-responsive nowrap table-striped align-middle" style="width: 100%;">
                <thead>
                    <tr>
                        <th scope="col" style="width: 10px;">
                            <div class="form-check">
                                <input class="form-check-input fs-15" type="checkbox" id="checkAll" value="option">
                            </div>
                        </th>
                        <th>SR No.</th>
                        <th>ID</th>
                        <th>Purchase ID</th>
                        <th>Title</th>
                        <th>User</th>
                        <th>Assigned To</th>
                        <th>Created By</th>
                        <th>Create Date</th>
                        <th>Status</th>
                        <th>Priority</th>
                        <th>Action</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <th scope="row">
                            <div class="form-check">
                                <input class="form-check-input fs-15" type="checkbox" name="checkAll" value="option1">
                            </div>
                        </th>
                        <td>01</td>
                        <td>VLZ-452</td>
                        <td>VLZ1400087402</td>
                        <td><a href="#!">Post launch reminder/ post list</a></td>
                        <td>Joseph Parker</td>
                        <td>Alexis Clarke</td>
                        <td>Joseph Parker</td>
                        <td>03 Oct, 2021</td>
                        <td><span class="badge badge-soft-info">Re-open</span></td>
                        <td><span class="badge bg-danger">High</span></td>
                        <td>
                            <div class="dropdown d-inline-block">
                                <button class="btn btn-soft-secondary btn-sm dropdown" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                    <i class="ri-more-fill align-middle"></i>
                                </button>
                                <ul class="dropdown-menu dropdown-menu-end">
                                    <li><a href="#!" class="dropdown-item"><i class="ri-eye-fill align-bottom me-2 text-muted"></i> View</a></li>
                                    <li><a class="dropdown-item edit-item-btn"><i class="ri-pencil-fill align-bottom me-2 text-muted"></i> Edit</a></li>
                                    <li>
                                        <a class="dropdown-item remove-item-btn">
                                            <i class="ri-delete-bin-fill align-bottom me-2 text-muted"></i> Delete
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th scope="row">
                            <div class="form-check">
                                <input class="form-check-input fs-15" type="checkbox" name="checkAll" value="option1">
                            </div>
                        </th>
                        <td>02</td>
                        <td>VLZ-453</td>
                        <td>VLZ1400087425</td>
                        <td><a href="#!">Additional Calendar</a></td>
                        <td>Diana Kohler</td>
                        <td>Admin</td>
                        <td>Mary Rucker</td>
                        <td>05 Oct, 2021</td>
                        <td><span class="badge badge-soft-secondary">On-Hold</span></td>
                        <td><span class="badge bg-info">Medium</span></td>
                        <td>
                            <div class="dropdown d-inline-block">
                                <button class="btn btn-soft-secondary btn-sm dropdown" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                    <i class="ri-more-fill align-middle"></i>
                                </button>
                                <ul class="dropdown-menu dropdown-menu-end">
                                    <li><a href="#!" class="dropdown-item"><i class="ri-eye-fill align-bottom me-2 text-muted"></i> View</a></li>
                                    <li><a class="dropdown-item edit-item-btn"><i class="ri-pencil-fill align-bottom me-2 text-muted"></i> Edit</a></li>
                                    <li>
                                        <a class="dropdown-item remove-item-btn">
                                            <i class="ri-delete-bin-fill align-bottom me-2 text-muted"></i> Delete
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th scope="row">
                            <div class="form-check">
                                <input class="form-check-input fs-15" type="checkbox" name="checkAll" value="option1">
                            </div>
                        </th>
                        <td>03</td>
                        <td>VLZ-454</td>
                        <td>VLZ1400087438</td>
                        <td><a href="#!">Make a creating an account profile</a></td>
                        <td>Tonya Noble</td>
                        <td>Admin</td>
                        <td>Tonya Noble</td>
                        <td>27 April, 2022</td>
                        <td><span class="badge badge-soft-danger">Closed</span></td>
                        <td><span class="badge bg-success">Low</span></td>
                        <td>
                            <div class="dropdown d-inline-block">
                                <button class="btn btn-soft-secondary btn-sm dropdown" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                    <i class="ri-more-fill align-middle"></i>
                                </button>
                                <ul class="dropdown-menu dropdown-menu-end">
                                    <li><a href="#!" class="dropdown-item"><i class="ri-eye-fill align-bottom me-2 text-muted"></i> View</a></li>
                                    <li><a class="dropdown-item edit-item-btn"><i class="ri-pencil-fill align-bottom me-2 text-muted"></i> Edit</a></li>
                                    <li>
                                        <a class="dropdown-item remove-item-btn">
                                            <i class="ri-delete-bin-fill align-bottom me-2 text-muted"></i> Delete
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th scope="row">
                            <div class="form-check">
                                <input class="form-check-input fs-15" type="checkbox" name="checkAll" value="option1">
                            </div>
                        </th>
                        <td>04</td>
                        <td>VLZ-455</td>
                        <td>VLZ1400087748</td>
                        <td><a href="#!">Apologize for shopping Error!</a></td>
                        <td>Joseph Parker</td>
                        <td>Alexis Clarke</td>
                        <td>Joseph Parker</td>
                        <td>14 June, 2021</td>
                        <td><span class="badge badge-soft-warning">Inprogress</span></td>
                        <td><span class="badge bg-info">Medium</span></td>
                        <td>
                            <div class="dropdown d-inline-block">
                                <button class="btn btn-soft-secondary btn-sm dropdown" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                    <i class="ri-more-fill align-middle"></i>
                                </button>
                                <ul class="dropdown-menu dropdown-menu-end">
                                    <li><a href="#!" class="dropdown-item"><i class="ri-eye-fill align-bottom me-2 text-muted"></i> View</a></li>
                                    <li><a class="dropdown-item edit-item-btn"><i class="ri-pencil-fill align-bottom me-2 text-muted"></i> Edit</a></li>
                                    <li>
                                        <a class="dropdown-item remove-item-btn">
                                            <i class="ri-delete-bin-fill align-bottom me-2 text-muted"></i> Delete
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th scope="row">
                            <div class="form-check">
                                <input class="form-check-input fs-15" type="checkbox" name="checkAll" value="option1">
                            </div>
                        </th>
                        <td>05</td>
                        <td>VLZ-456</td>
                        <td>VLZ1400087547</td>
                        <td><a href="#!">Support for theme</a></td>
                        <td>Donald Palmer</td>
                        <td>Admin</td>
                        <td>Donald Palmer</td>
                        <td>25 June, 2021</td>
                        <td><span class="badge badge-soft-danger">Closed</span></td>
                        <td><span class="badge bg-success">Low</span></td>
                        <td>
                            <div class="dropdown d-inline-block">
                                <button class="btn btn-soft-secondary btn-sm dropdown" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                    <i class="ri-more-fill align-middle"></i>
                                </button>
                                <ul class="dropdown-menu dropdown-menu-end">
                                    <li><a href="#!" class="dropdown-item"><i class="ri-eye-fill align-bottom me-2 text-muted"></i> View</a></li>
                                    <li><a class="dropdown-item edit-item-btn"><i class="ri-pencil-fill align-bottom me-2 text-muted"></i> Edit</a></li>
                                    <li>
                                        <a class="dropdown-item remove-item-btn">
                                            <i class="ri-delete-bin-fill align-bottom me-2 text-muted"></i> Delete
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th scope="row">
                            <div class="form-check">
                                <input class="form-check-input fs-15" type="checkbox" name="checkAll" value="option1">
                            </div>
                        </th>
                        <td>06</td>
                        <td>VLZ-457</td>
                        <td>VLZ1400087245</td>
                        <td><a href="#!">Benner design for FB & Twitter</a></td>
                        <td>Mary Rucker</td>
                        <td>Jennifer Carter</td>
                        <td>Mary Rucker</td>
                        <td>14 Aug, 2021</td>
                        <td><span class="badge badge-soft-warning">Inprogress</span></td>
                        <td><span class="badge bg-info">Medium</span></td>
                        <td>
                            <div class="dropdown d-inline-block">
                                <button class="btn btn-soft-secondary btn-sm dropdown" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                    <i class="ri-more-fill align-middle"></i>
                                </button>
                                <ul class="dropdown-menu dropdown-menu-end">
                                    <li><a href="#!" class="dropdown-item"><i class="ri-eye-fill align-bottom me-2 text-muted"></i> View</a></li>
                                    <li><a class="dropdown-item edit-item-btn"><i class="ri-pencil-fill align-bottom me-2 text-muted"></i> Edit</a></li>
                                    <li>
                                        <a class="dropdown-item remove-item-btn">
                                            <i class="ri-delete-bin-fill align-bottom me-2 text-muted"></i> Delete
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th scope="row">
                            <div class="form-check">
                                <input class="form-check-input fs-15" type="checkbox" name="checkAll" value="option1">
                            </div>
                        </th>
                        <td>07</td>
                        <td>VLZ-458</td>
                        <td>VLZ1400087785</td>
                        <td><a href="#!">Change email option process</a></td>
                        <td>James Morris</td>
                        <td>Admin</td>
                        <td>James Morris</td>
                        <td>12 March, 2022</td>
                        <td><span class="badge badge-soft-primary">Open</span></td>
                        <td><span class="badge bg-danger">High</span></td>
                        <td>
                            <div class="dropdown d-inline-block">
                                <button class="btn btn-soft-secondary btn-sm dropdown" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                    <i class="ri-more-fill align-middle"></i>
                                </button>
                                <ul class="dropdown-menu dropdown-menu-end">
                                    <li><a href="#!" class="dropdown-item"><i class="ri-eye-fill align-bottom me-2 text-muted"></i> View</a></li>
                                    <li><a class="dropdown-item edit-item-btn"><i class="ri-pencil-fill align-bottom me-2 text-muted"></i> Edit</a></li>
                                    <li>
                                        <a class="dropdown-item remove-item-btn">
                                            <i class="ri-delete-bin-fill align-bottom me-2 text-muted"></i> Delete
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th scope="row">
                            <div class="form-check">
                                <input class="form-check-input fs-15" type="checkbox" name="checkAll" value="option1">
                            </div>
                        </th>
                        <td>08</td>
                        <td>VLZ-460</td>
                        <td>VLZ1400087745</td>
                        <td><a href="#!">Support for theme</a></td>
                        <td>Nathan Cole</td>
                        <td>Nancy Martino</td>
                        <td>Nathan Cole</td>
                        <td>28 Feb, 2022</td>
                        <td><span class="badge badge-soft-secondary">On-Hold</span></td>
                        <td><span class="badge bg-success">Low</span></td>
                        <td>
                            <div class="dropdown d-inline-block">
                                <button class="btn btn-soft-secondary btn-sm dropdown" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                    <i class="ri-more-fill align-middle"></i>
                                </button>
                                <ul class="dropdown-menu dropdown-menu-end">
                                    <li><a href="#!" class="dropdown-item"><i class="ri-eye-fill align-bottom me-2 text-muted"></i> View</a></li>
                                    <li><a class="dropdown-item edit-item-btn"><i class="ri-pencil-fill align-bottom me-2 text-muted"></i> Edit</a></li>
                                    <li>
                                        <a class="dropdown-item remove-item-btn">
                                            <i class="ri-delete-bin-fill align-bottom me-2 text-muted"></i> Delete
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th scope="row">
                            <div class="form-check">
                                <input class="form-check-input fs-15" type="checkbox" name="checkAll" value="option1">
                            </div>
                        </th>
                        <td>09</td>
                        <td>VLZ-461</td>
                        <td>VLZ1400087179</td>
                        <td><a href="#!">Form submit issue</a></td>
                        <td>Grace Coles</td>
                        <td>Admin</td>
                        <td>Grace Coles</td>
                        <td>07 Jan, 2022</td>
                        <td><span class="badge badge-soft-success">New</span></td>
                        <td><span class="badge bg-danger">High</span></td>
                        <td>
                            <div class="dropdown d-inline-block">
                                <button class="btn btn-soft-secondary btn-sm dropdown" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                    <i class="ri-more-fill align-middle"></i>
                                </button>
                                <ul class="dropdown-menu dropdown-menu-end">
                                    <li><a href="#!" class="dropdown-item"><i class="ri-eye-fill align-bottom me-2 text-muted"></i> View</a></li>
                                    <li><a class="dropdown-item edit-item-btn"><i class="ri-pencil-fill align-bottom me-2 text-muted"></i> Edit</a></li>
                                    <li>
                                        <a class="dropdown-item remove-item-btn">
                                            <i class="ri-delete-bin-fill align-bottom me-2 text-muted"></i> Delete
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th scope="row">
                            <div class="form-check">
                                <input class="form-check-input fs-15" type="checkbox" name="checkAll" value="option1">
                            </div>
                        </th>
                        <td>10</td>
                        <td>VLZ-462</td>
                        <td>VLZ140008856</td>
                        <td><a href="#!">Edit customer testimonial</a></td>
                        <td>Freda</td>
                        <td>Alexis Clarke</td>
                        <td>Freda</td>
                        <td>16 Aug, 2021</td>
                        <td><span class="badge badge-soft-danger">Closed</span></td>
                        <td><span class="badge bg-info">Medium</span></td>
                        <td>
                            <div class="dropdown d-inline-block">
                                <button class="btn btn-soft-secondary btn-sm dropdown" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                    <i class="ri-more-fill align-middle"></i>
                                </button>
                                <ul class="dropdown-menu dropdown-menu-end">
                                    <li><a href="#!" class="dropdown-item"><i class="ri-eye-fill align-bottom me-2 text-muted"></i> View</a></li>
                                    <li><a class="dropdown-item edit-item-btn"><i class="ri-pencil-fill align-bottom me-2 text-muted"></i> Edit</a></li>
                                    <li>
                                        <a class="dropdown-item remove-item-btn">
                                            <i class="ri-delete-bin-fill align-bottom me-2 text-muted"></i> Delete
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th scope="row">
                            <div class="form-check">
                                <input class="form-check-input fs-15" type="checkbox" name="checkAll" value="option1">
                            </div>
                        </th>
                        <td>11</td>
                        <td>VLZ-463</td>
                        <td>VLZ1400078031</td>
                        <td><a href="#!">Ca i have an e-copy invoice</a></td>
                        <td>Williams</td>
                        <td>Admin</td>
                        <td>Williams</td>
                        <td>24 Feb, 2022</td>
                        <td><span class="badge badge-soft-primary">Open</span></td>
                        <td><span class="badge bg-success">Low</span></td>
                        <td>
                            <div class="dropdown d-inline-block">
                                <button class="btn btn-soft-secondary btn-sm dropdown" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                    <i class="ri-more-fill align-middle"></i>
                                </button>
                                <ul class="dropdown-menu dropdown-menu-end">
                                    <li><a href="#!" class="dropdown-item"><i class="ri-eye-fill align-bottom me-2 text-muted"></i> View</a></li>
                                    <li><a class="dropdown-item edit-item-btn"><i class="ri-pencil-fill align-bottom me-2 text-muted"></i> Edit</a></li>
                                    <li>
                                        <a class="dropdown-item remove-item-btn">
                                            <i class="ri-delete-bin-fill align-bottom me-2 text-muted"></i> Delete
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th scope="row">
                            <div class="form-check">
                                <input class="form-check-input fs-15" type="checkbox" name="checkAll" value="option1">
                            </div>
                        </th>
                        <td>12</td>
                        <td>VLZ-464</td>
                        <td>VLZ1400087416</td>
                        <td><a href="#!">Brand logo design</a></td>
                        <td>Richard V.</td>
                        <td>Admin</td>
                        <td>Richard V.</td>
                        <td>16 March, 2021</td>
                        <td><span class="badge badge-soft-warning">Inprogress</span></td>
                        <td><span class="badge bg-danger">High</span></td>
                        <td>
                            <div class="dropdown d-inline-block">
                                <button class="btn btn-soft-secondary btn-sm dropdown" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                    <i class="ri-more-fill align-middle"></i>
                                </button>
                                <ul class="dropdown-menu dropdown-menu-end">
                                    <li><a href="#!" class="dropdown-item"><i class="ri-eye-fill align-bottom me-2 text-muted"></i> View</a></li>
                                    <li><a class="dropdown-item edit-item-btn"><i class="ri-pencil-fill align-bottom me-2 text-muted"></i> Edit</a></li>
                                    <li>
                                        <a class="dropdown-item remove-item-btn">
                                            <i class="ri-delete-bin-fill align-bottom me-2 text-muted"></i> Delete
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th scope="row">
                            <div class="form-check">
                                <input class="form-check-input fs-15" type="checkbox" name="checkAll" value="option1">
                            </div>
                        </th>
                        <td>13</td>
                        <td>VLZ-466</td>
                        <td>VLZ1400089015</td>
                        <td><a href="#!">Issue with finding information about order ?</a></td>
                        <td>Olive Gunther</td>
                        <td>Alexis Clarke</td>
                        <td>Schaefer</td>
                        <td>32 March, 2022</td>
                        <td><span class="badge badge-soft-success">New</span></td>
                        <td><span class="badge bg-danger">High</span></td>
                        <td>
                            <div class="dropdown d-inline-block">
                                <button class="btn btn-soft-secondary btn-sm dropdown" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                    <i class="ri-more-fill align-middle"></i>
                                </button>
                                <ul class="dropdown-menu dropdown-menu-end">
                                    <li><a href="#!" class="dropdown-item"><i class="ri-eye-fill align-bottom me-2 text-muted"></i> View</a></li>
                                    <li><a class="dropdown-item edit-item-btn"><i class="ri-pencil-fill align-bottom me-2 text-muted"></i> Edit</a></li>
                                    <li>
                                        <a class="dropdown-item remove-item-btn">
                                            <i class="ri-delete-bin-fill align-bottom me-2 text-muted"></i> Delete
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th scope="row">
                            <div class="form-check">
                                <input class="form-check-input fs-15" type="checkbox" name="checkAll" value="option1">
                            </div>
                        </th>
                        <td>14</td>
                        <td>VLZ-467</td>
                        <td>VLZ1400090324</td>
                        <td><a href="#!">Make a creating an account profile</a></td>
                        <td>Edwin</td>
                        <td>Admin</td>
                        <td>Edwin</td>
                        <td>05 April, 2022</td>
                        <td><span class="badge badge-soft-warning">Inprogress</span></td>
                        <td><span class="badge bg-success">Low</span></td>
                        <td>
                            <div class="dropdown d-inline-block">
                                <button class="btn btn-soft-secondary btn-sm dropdown" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                    <i class="ri-more-fill align-middle"></i>
                                </button>
                                <ul class="dropdown-menu dropdown-menu-end">
                                    <li><a href="#!" class="dropdown-item"><i class="ri-eye-fill align-bottom me-2 text-muted"></i> View</a></li>
                                    <li><a class="dropdown-item edit-item-btn"><i class="ri-pencil-fill align-bottom me-2 text-muted"></i> Edit</a></li>
                                    <li>
                                        <a class="dropdown-item remove-item-btn">
                                            <i class="ri-delete-bin-fill align-bottom me-2 text-muted"></i> Delete
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>`;
        } else {
            let units = Object.keys(searchBoxObj).length;

            if (units <= 4) {
                let columns = 12 / units;

                html += '<form class="mb-5">';
                html += '<div class="row mb-lg-3">';

                Object.entries(searchBoxObj).map(([key, value], index) => {
                    html += `
                        <div class="col-md-${columns} mb-lg-0 mb-3">
                            <label>${value.fullName}</label>
                            <input type="text" class="form-control datatable-${uniqueId}-input" placeholder="" data-col-index="${value.index}" />
                        </div>
                    `;

                    return '';
                });

                html += '</div>';
                html += `
                    <div class="row mt-4">
                        <div class="col-lg-12">
                            <button class="btn btn-primary btn-primary--icon" id="${uniqueId}_search">
                                <span>
                                    <i class="la la-search"></i>
                                    <span>Search</span>
                                </span>
                            </button>&#160;&#160;
                            <button class="btn btn-secondary btn-secondary--icon" id="${uniqueId}_reset">
                                <span>
                                    <i class="la la-close"></i>
                                    <span>Reset</span>
                                </span>
                            </button>
                        </div>
                    </div>
                `;
                html += '</form>';
            } else {
                let mod = units % 4;
                let columns = units - mod;
                let rest = 12 / mod;
                let reassign = { };

                html += '<form class="mb-5">';

                let b = 0;

                Object.entries(searchBoxObj).map(([key, value], index) => {
                    if (index % 4 === 0) {
                        b++;
                        reassign[b] = [];
                        reassign[b].push({ key, value });
                    } 
                    else {
                        reassign[b].push({ key, value });
                    }

                    // if (index < columns) {
                    //     html += `
                    //         <div class="col-md-3 mb-lg-0 mb-3">
                    //             <label>${value.fullName}</label>
                    //             <input type="text" class="form-control datatable-input" placeholder="" data-col-index="${value.index}" />
                    //         </div>
                    //     `;
                    // }
                    
                    // if (index >= columns) {
                    //     html += `
                    //         <div class="col-md-${rest} mb-lg-0 mb-3">
                    //             <label>${value.fullName}</label>
                    //             <input type="text" class="form-control datatable-input" placeholder="" data-col-index="${value.index}" />
                    //         </div>
                    //     `;            
                    // }

                    return '';
                });

                Object.entries(reassign).map(([key, value], index) => {
                    html += '<div class="row mb-lg-3">';

                    if (value.length === 4) {
                        value.forEach((val, index) => {
                            html += `
                                <div class="col-md-3 mb-lg-0 mb-3">
                                    <label>${val.value.fullName}</label>
                                    <input type="text" class="form-control datatable-${uniqueId}-input" placeholder="" data-col-index="${val.value.index}" />
                                </div>
                            `;
                        });
                    }
                    else {
                        value.forEach((val, index) => {
                            html += `
                                <div class="col-md-${rest} mb-lg-0 mb-3">
                                    <label>${val.value.fullName}</label>
                                    <input type="text" class="form-control datatable-${uniqueId}-input" placeholder="" data-col-index="${val.value.index}" />
                                </div>
                            `;
                        });  
                    }

                    html += '</div>';
                    return '';
                });

                html += `
                    <div class="row mt-4">
                        <div class="col-lg-12">
                            <button class="btn btn-primary btn-primary--icon" id="${uniqueId}_search">
                                <span>
                                    <i class="la la-search"></i>
                                    <span>Search</span>
                                </span>
                            </button>&#160;&#160;
                            <button class="btn btn-secondary btn-secondary--icon" id="${uniqueId}_reset">
                                <span>
                                    <i class="la la-close"></i>
                                    <span>Reset</span>
                                </span>
                            </button>
                        </div>
                    </div>
                `;
                html += '</form>';           
            }

            html += `<table id="${uniqueId}" class="table table-bordered dt-responsive nowrap table-striped align-middle" style="width: 100%;">
                <thead>
                    <tr>
                        ${columnsHTML}
                    </tr>
                </thead>
            </table>`;
        }

        return html;
    }

    useEffect(() => {
        const attrsRow = `class='gjs-row' data-gjs-type='rowTest' data-gjs-droppable='.gjs-cell' data-gjs-resizable='{"tl":0,"tc":0,"tr":0,"cl":0,"cr":0,"bl":0,"br":0,"minDim":1}' data-gjs-name='Row'`;
        const attrsCell = `class='gjs-cell' data-gjs-type='colTest' data-gjs-draggable='.gjs-row' data-gjs-resizable='{"tl":0,"tc":0,"tr":0,"cl":0,"cr":1,"bl":0,"br":0,"minDim":1,"bc":0,"currentUnit":1,"step":0.2}' data-gjs-name='Cell'`;
        const attrsCell2 = `class='gjs-cell2' data-gjs-type='colTest' data-gjs-draggable='.gjs-row' data-gjs-resizable='{"tl":0,"tc":0,"tr":0,"cl":0,"cr":1,"bl":0,"br":0,"minDim":1,"bc":0,"currentUnit":1,"step":0.2}' data-gjs-name='Cell'`;
        const attrsCell3 = `class='gjs-cell3' data-gjs-type='colTest' data-gjs-draggable='.gjs-row' data-gjs-resizable='{"tl":0,"tc":0,"tr":0,"cl":0,"cr":1,"bl":0,"br":0,"minDim":1,"bc":0,"currentUnit":1,"step":0.2}' data-gjs-name='Cell'`;
        const styleRow = `
            .gjs-row { display: table; padding: 10px; width: 100%; }
            @media (max-width: 768px) { 
                .gjs-cell, .gjs-cell30, .gjs-cell70 {
                    width: 100%;
                    display: block;
                }
            }`;
    
        const styleClm = `.gjs-cell { width: 8%; display: table-cell; height: 75px; }`;
        const styleClm2 = `.gjs-cell2 { width: 50%; display: table-cell; height: 75px; }`;
        const styleClm3 = `.gjs-cell3 { width: 33.3333%; display: table-cell; height: 75px; }`;
        const styleClm30 = `.gjs-cell30 { width: 30%; }`;
        const styleClm70 = `.gjs-cell70 { width: 70%; }`;

        if (projectData === null && componentsData === null) {
            axios.get("/DashboardBuilderDatas/GetDashboardBuilderData?id=1").then(response => {
                setProjectData(JSON.parse(response.projectData));
                setComponentsData(JSON.parse(response.componentsData));
            });
        }

        if (apiLists === null) {
            axios.get("/ApiLists/GetApiLists").then((response) => {
                if (response.length > 0) {
                    let grouped = _.reduce(response, (result, res) => {
                        (result[res.type] || (result[res.type] = [])).push(res);
                        return result;
                    }, {});

                    setApiLists(grouped);
                }
            });
        }

        if (widgetBoxColumnLists === null && widgetBoxDescriptionLists === null) {
            axios.get("/WidgetBoxes/GetWidgetBoxColumnDescriptionLists").then((response) => {
                if (response["columns"].length > 0) {
                    setWidgetBoxColumnLists(response["columns"]);
                }
                if (response["rows"].length > 0) {
                    setWidgetBoxDescriptionLists(response["rows"]);
                }
            });
        }

        if (mssqlTable === null) {
            axios.get("/DataTable/GetMSSQLTable").then((response) => {
                setMssqlTable(response);
            });
        }

        if (projectData && apiLists && apiLists && widgetBoxColumnLists && widgetBoxDescriptionLists && mssqlTable) {
            let barChartWebApiLists = {};
            let lineChartWebApiLists = {};
            let pieChartWebApiLists = {};
            let gaugeChartWebApiLists = {};

            if (apiLists['bar'].length > 0) {
                barChartWebApiLists = apiLists['bar'];
            }
            
            if (apiLists['gauge'].length > 0) {
                gaugeChartWebApiLists = apiLists['gauge'];
            }
            
            if (apiLists['line'].length > 0) {
                lineChartWebApiLists = apiLists['line'];
            }
            
            if (apiLists['pie'].length > 0) {
                pieChartWebApiLists = apiLists['pie'];
            }            

            var editor = grapesjs.init({
                height: '100%',
                container : '#gjs',
                // fromElement: true,
                storageManager: false,
                allowScripts: 1,
                blockManager: { 
                    blocks: []
                },
                canvas: { 
                    styles: [
                        'https://cdn.datatables.net/1.11.5/css/dataTables.bootstrap5.min.css',
                        'https://cdn.datatables.net/responsive/2.2.9/css/responsive.bootstrap.min.css',
                        'https://cdn.datatables.net/buttons/2.2.2/css/buttons.dataTables.min.css',
                        'assets/js/layout.js',
                        'assets/css/bootstrap.min.css',
                        'assets/css/icons.min.css',
                        'assets/css/app.min.css',
                        'assets/css/custom.min.css'
                    ],
                    scripts: [
                        'lib/global-script.js',
                        'lib/chart.js',
                        'lib/jquery-3.6.3.min.js',
                        'https://cdn.datatables.net/1.11.5/js/jquery.dataTables.min.js',
                        'https://cdn.datatables.net/1.11.5/js/dataTables.bootstrap5.min.js',
                        'https://cdn.datatables.net/responsive/2.2.9/js/dataTables.responsive.min.js',
                        'https://cdn.datatables.net/buttons/2.2.2/js/dataTables.buttons.min.js',
                        'https://cdn.datatables.net/buttons/2.2.2/js/buttons.print.min.js',
                        'https://cdn.datatables.net/buttons/2.2.2/js/buttons.html5.min.js',
                        'https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.53/vfs_fonts.js',
                        'https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.53/pdfmake.min.js',
                        'https://cdnjs.cloudflare.com/ajax/libs/jszip/3.1.3/jszip.min.js'
                    ],
                },
                plugins: [],
                pluginsOpts: {},
                // modal: { custom: true },
            });
    
            // Register Type
            editor.Components.addType('rowTest', {
    
            });

            editor.Components.addType('barChart', {
    
            });
    
            editor.Components.addType('pieChart', {
    
            });
    
            editor.Components.addType('lineChart', {
    
            });
    
            editor.Components.addType('gaugeChart', {
    
            });

            editor.Components.addType('widgetBox', {
    
            });

            editor.Components.addType('dataTable', {
    
            });
            
            editor.BlockManager.add('column1',
                {
                    category : 'Basic',
                    label: '1 Column',
                    media: `<svg viewBox="0 0 24 24">
                        <path fill="currentColor" d="M2 20h20V4H2v16Zm-1 0V4a1 1 0 0 1 1-1h20a1 1 0 0 1 1 1v16a1 1 0 0 1-1 1H2a1 1 0 0 1-1-1Z"/>
                    </svg>`,
                    content: 
                        // `<div ${attrsRow}>
                        //     <div ${attrsCell}></div>
                        // </div>
                        // <style>
                        //     ${styleRow}
                        //     ${styleClm}
                        // </style>`
                        `<div class="row" data-gjs-type="containerRow" data-gjs-name="Row" style="padding: 8px; width: 100%; margin-right: auto; margin-left: auto;">
                            <div class="col" data-gjs-type="containerColumn" data-gjs-name="Column" style="padding: 8px; height: 75px;"></div>
                        </div>`
                }
            );
    
            editor.BlockManager.add('column2',
                {
                    category : 'Basic',
                    label: '2 Column',
                    media: `<svg viewBox="0 0 23 24">
                        <path fill="currentColor" d="M2 20h8V4H2v16Zm-1 0V4a1 1 0 0 1 1-1h8a1 1 0 0 1 1 1v16a1 1 0 0 1-1 1H2a1 1 0 0 1-1-1ZM13 20h8V4h-8v16Zm-1 0V4a1 1 0 0 1 1-1h8a1 1 0 0 1 1 1v16a1 1 0 0 1-1 1h-8a1 1 0 0 1-1-1Z"/>
                    </svg>`,
                    content: 
                        // `<div ${attrsRow}>
                        //     <div ${attrsCell2}></div>
                        //     <div ${attrsCell2}></div>
                        // </div>
                        // <style>
                        //     ${styleRow}
                        //     ${styleClm2}
                        // </style>`
                        `<div class="row" data-gjs-type="containerRow" data-gjs-name="Row" style="padding: 8px; width: 100%; margin-right: auto; margin-left: auto;">
                            <div class="col-6" data-gjs-type="containerColumn" data-gjs-name="Column" style="padding: 8px; height: 75px;"></div>
                            <div class="col-6" data-gjs-type="containerColumn" data-gjs-name="Column" style="padding: 8px; height: 75px;"></div>
                        </div>`
                }
            );
    
            editor.BlockManager.add('column3', {
                category : 'Basic',
                label: '3 Columns',
                media: `<svg viewBox="0 0 23 24">
                    <path fill="currentColor" d="M2 20h4V4H2v16Zm-1 0V4a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v16a1 1 0 0 1-1 1H2a1 1 0 0 1-1-1ZM17 20h4V4h-4v16Zm-1 0V4a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v16a1 1 0 0 1-1 1h-4a1 1 0 0 1-1-1ZM9.5 20h4V4h-4v16Zm-1 0V4a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v16a1 1 0 0 1-1 1h-4a1 1 0 0 1-1-1Z"/>
                </svg>`,
                content: 
                    // `<div ${attrsRow}>
                    //     <div ${attrsCell3}></div>
                    //     <div ${attrsCell3}></div>
                    //     <div ${attrsCell3}></div>
                    // </div>
                    // <style>
                    //     ${styleRow}
                    //     ${styleClm3}
                    // </style>`
                `<div class="row" data-gjs-type="containerRow" data-gjs-name="Row" style="padding: 8px; width: 100%; margin-right: auto; margin-left: auto;">
                    <div class="col-4" data-gjs-type="containerColumn" data-gjs-name="Column" style="padding: 8px; height: 75px;"></div>
                    <div class="col-4" data-gjs-type="containerColumn" data-gjs-name="Column" style="padding: 8px; height: 75px;"></div>
                    <div class="col-4" data-gjs-type="containerColumn" data-gjs-name="Column" style="padding: 8px; height: 75px;"></div>
                </div>`
            });

            editor.BlockManager.add('column4', {
                category : 'Basic',
                label: '4 Columns',
                media: `<svg version="1.1" id="Layer_1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px"
                viewBox="0 0 512 512" style="enable-background:new 0 0 512 512;" xml:space="preserve">
           <g id="XMLID_1_">
               <polygon id="XMLID_3_" points="8.4,8.4 8.4,15.8 120.1,15.8 120.1,496.2 15.8,496.2 15.8,8.4 8.4,8.4 8.4,15.8 8.4,8.4 0,8.4 
                   0,512 135.9,512 135.9,0 0,0 0,8.4 	"/>
               <polygon id="XMLID_4_" points="128.5,8.4 128.5,15.8 247.6,15.8 247.6,496.2 135.9,496.2 135.9,8.4 128.5,8.4 128.5,15.8 
                   128.5,8.4 120.1,8.4 120.1,512 264.4,512 264.4,0 120.1,0 120.1,8.4 	"/>
               <polygon id="XMLID_5_" points="256,8.4 256,15.8 367.7,15.8 367.7,496.2 264.4,496.2 264.4,8.4 256,8.4 256,15.8 256,8.4 
                   247.6,8.4 247.6,512 383.5,512 383.5,0 247.6,0 247.6,8.4 	"/>
               <polygon id="XMLID_6_" points="376.1,8.4 376.1,15.8 496.2,15.8 496.2,496.2 383.5,496.2 383.5,8.4 376.1,8.4 376.1,15.8 
                   376.1,8.4 367.7,8.4 367.7,512 512,512 512,0 367.7,0 367.7,8.4 	"/>
           </g>
           </svg>`,
                content: 
                    // `<div ${attrsRow}>
                    //     <div ${attrsCell3}></div>
                    //     <div ${attrsCell3}></div>
                    //     <div ${attrsCell3}></div>
                    // </div>
                    // <style>
                    //     ${styleRow}
                    //     ${styleClm3}
                    // </style>`
                `<div class="row" data-gjs-type="containerRow" data-gjs-name="Row" style="padding: 8px; width: 100%; margin-right: auto; margin-left: auto;">
                    <div class="col-3" data-gjs-type="containerColumn" data-gjs-name="Column" style="padding: 8px; height: 75px;"></div>
                    <div class="col-3" data-gjs-type="containerColumn" data-gjs-name="Column" style="padding: 8px; height: 75px;"></div>
                    <div class="col-3" data-gjs-type="containerColumn" data-gjs-name="Column" style="padding: 8px; height: 75px;"></div>
                    <div class="col-3" data-gjs-type="containerColumn" data-gjs-name="Column" style="padding: 8px; height: 75px;"></div>
                </div>`
            });
    
            editor.BlockManager.add('column3-7', {
                category : 'Basic',
                label: '2 Columns 3/7',
                media: `<svg viewBox="0 0 24 24">
                    <path fill="currentColor" d="M2 20h5V4H2v16Zm-1 0V4a1 1 0 0 1 1-1h5a1 1 0 0 1 1 1v16a1 1 0 0 1-1 1H2a1 1 0 0 1-1-1ZM10 20h12V4H10v16Zm-1 0V4a1 1 0 0 1 1-1h12a1 1 0 0 1 1 1v16a1 1 0 0 1-1 1H10a1 1 0 0 1-1-1Z"/>
                </svg>`,
                content: 
                    // `<div ${attrsRow}>
                    //     <div ${attrsCell} style='width: 30%;'></div>
                    //     <div ${attrsCell} style='width: 70%;'></div>
                    // </div>
                    // <style>
                    //     ${styleRow}
                    //     ${styleClm}
                    //     ${styleClm30}
                    //     ${styleClm70}
                    // </style>`
                    `<div class="row" data-gjs-type="containerRow" data-gjs-name="Row" style="padding: 8px; width: 100%; margin-right: auto; margin-left: auto;">
                        <div class="col-4" data-gjs-type="containerColumn" data-gjs-name="Column" style="padding: 8px; height: 75px;"></div>
                        <div class="col-8" data-gjs-type="containerColumn" data-gjs-name="Column" style="padding: 8px; height: 75px;"></div>
                    </div>`
            });
    
            editor.BlockManager.add('text', {
                category : 'Basic',
                label: 'Text',
                media: `<svg viewBox="0 0 24 24">
                    <path fill="currentColor" d="M18.5,4L19.66,8.35L18.7,8.61C18.25,7.74 17.79,6.87 17.26,6.43C16.73,6 16.11,6 15.5,6H13V16.5C13,17 13,17.5 13.33,17.75C13.67,18 14.33,18 15,18V19H9V18C9.67,18 10.33,18 10.67,17.75C11,17.5 11,17 11,16.5V6H8.5C7.89,6 7.27,6 6.74,6.43C6.21,6.87 5.75,7.74 5.3,8.61L4.34,8.35L5.5,4H18.5Z" />
                </svg>`,
                content: {
                    type: 'text',
                    content: 'Insert your text here',
                    style: { padding: '10px' },
                }
            });
    
            editor.BlockManager.add('pieChart', {
                category : 'Chart JS',
                label: 'Pie Chart',
                media: `<svg version="1.0" xmlns="http://www.w3.org/2000/svg"
                    viewBox="0 0 100.000000 100.000000"
                    preserveAspectRatio="xMidYMid meet">
                    <g transform="translate(0.000000,100.000000) scale(0.100000,-0.100000)"
                    fill="currentColor" stroke="none">
                    <path d="M350 944 c-106 -28 -197 -98 -253 -195 -29 -49 -57 -146 -57 -196 l0
                    -33 220 0 220 0 0 220 0 220 -37 -1 c-21 0 -63 -7 -93 -15z m90 -204 l0 -180
                    -181 0 -181 0 7 33 c23 115 76 198 165 259 47 32 134 65 178 67 9 1 12 -41 12
                    -179z"/>
                    <path d="M520 863 c0 -17 8 -21 53 -26 94 -10 166 -44 233 -111 81 -81 108
                    -148 108 -266 0 -118 -27 -185 -108 -266 -81 -81 -148 -108 -266 -108 -117 0
                    -182 26 -265 108 -68 67 -102 138 -112 234 -5 44 -9 52 -26 52 -18 0 -19 -5
                    -14 -59 17 -161 117 -293 267 -353 78 -31 222 -31 300 0 112 45 197 130 242
                    242 31 78 31 222 0 300 -60 150 -192 250 -353 267 -54 5 -59 4 -59 -14z"/>
                    </g>
                </svg>`,
                content:        
                {
                    type: 'pieChart',
                    tagName: "canvas",
                    droppable: false,
                    copyable: false,
                    attributes: {
                        name: 'pie-chart',
                        firstCreated: true,
                    },
                }
            });
    
            editor.BlockManager.add('barChart', {
                category : 'Chart JS',
                label: 'Bar Chart',
                media: `<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" viewBox="0 0 256 256" xml:space="preserve">
                    <defs></defs>
                    <g fill="currentColor" transform="translate(1.4065934065934016 1.4065934065934016) scale(2.81 2.81)" >
                        <path d="M 22.801 90 H 9.875 c -2.461 0 -4.463 -2.002 -4.463 -4.463 V 58.453 c 0 -2.461 2.002 -4.463 4.463 -4.463 h 12.926 c 2.461 0 4.464 2.002 4.464 4.463 v 27.084 C 27.265 87.998 25.263 90 22.801 90 z M 9.875 56.957 c -0.825 0 -1.496 0.671 -1.496 1.496 v 27.084 c 0 0.825 0.671 1.496 1.496 1.496 h 12.926 c 0.825 0 1.497 -0.671 1.497 -1.496 V 58.453 c 0 -0.825 -0.671 -1.496 -1.497 -1.496 H 9.875 z" />
                        <path d="M 51.463 90 H 38.537 c -2.461 0 -4.464 -2.002 -4.464 -4.463 V 40.279 c 0 -2.461 2.002 -4.464 4.464 -4.464 h 12.926 c 2.461 0 4.463 2.002 4.463 4.464 v 45.257 C 55.926 87.998 53.924 90 51.463 90 z M 38.537 38.783 c -0.825 0 -1.497 0.671 -1.497 1.497 v 45.257 c 0 0.825 0.671 1.496 1.497 1.496 h 12.926 c 0.825 0 1.496 -0.671 1.496 -1.496 V 40.279 c 0 -0.825 -0.671 -1.497 -1.496 -1.497 H 38.537 z" />
                        <path d="M 80.124 90 H 67.198 c -2.461 0 -4.463 -2.002 -4.463 -4.463 V 4.464 C 62.735 2.002 64.737 0 67.198 0 h 12.926 c 2.462 0 4.464 2.002 4.464 4.464 v 81.073 C 84.588 87.998 82.586 90 80.124 90 z M 67.198 2.967 c -0.825 0 -1.496 0.671 -1.496 1.497 v 81.073 c 0 0.825 0.671 1.496 1.496 1.496 h 12.926 c 0.826 0 1.497 -0.671 1.497 -1.496 V 4.464 c 0 -0.825 -0.671 -1.497 -1.497 -1.497 H 67.198 z" />
                    </g>
                </svg>`,
                content:        
                    {
                        type: 'barChart',
                        tagName: "canvas",
                        droppable: false,
                        copyable: false,
                        attributes: {
                            name: 'bar-chart',
                            firstCreated: true,
                        },
                    }
            });
    
            editor.BlockManager.add('lineChart', {
                category : 'Chart JS',
                label: 'Line Chart',
                media: `<svg fill="currentColor" version="1.1" id="Layer_1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 512 512" xml:space="preserve">
                    <g>
                        <g>
                            <path d="M94.398,24.711L71.845,2.44c-3.294-3.253-8.591-3.253-11.885,0L37.406,24.711c-3.324,3.282-3.358,8.637-0.076,11.962
                                c3.283,3.322,8.637,3.358,11.961,0.074l8.153-8.052v73.53c0,4.672,3.787,8.458,8.458,8.458s8.458-3.786,8.458-8.458V28.697
                                l8.153,8.052c1.648,1.627,3.796,2.439,5.942,2.439c2.183,0,4.364-0.839,6.018-2.515C97.756,33.349,97.722,27.994,94.398,24.711z"
                                />
                        </g>
                    </g>
                    <g>
                        <g>
                            <path d="M509.561,314.625l-22.271-22.554c-1.642-1.662-3.801-2.502-5.967-2.516c-0.541-0.003-1.083,0.045-1.616,0.144
                                c-1.068,0.2-2.104,0.607-3.046,1.22c-0.47,0.307-0.918,0.665-1.333,1.076c-0.624,0.616-1.131,1.304-1.522,2.038
                                c-0.262,0.489-0.471,0.999-0.629,1.521s-0.265,1.057-0.32,1.596c-0.138,1.348,0.047,2.721,0.555,3.997
                                c0.304,0.766,0.726,1.495,1.263,2.164c0.179,0.222,0.371,0.439,0.576,0.646l8.052,8.153H312.091l-60.224-129.323
                                c-1.272-2.734-3.904-4.582-6.906-4.854c-3.006-0.27-5.923,1.077-7.664,3.536l-25.088,35.436l-42.646-93.417
                                c-1.354-2.966-4.292-4.89-7.551-4.945c-3.236-0.08-6.26,1.768-7.713,4.687l-29.955,60.156c-2.082,4.18-0.38,9.258,3.802,11.341
                                c4.181,2.082,9.258,0.381,11.341-3.801l22.053-44.285l41.388,90.661c1.256,2.753,3.888,4.626,6.901,4.909
                                c3.016,0.288,5.948-1.065,7.695-3.533l25.161-35.541l50.748,108.974H79.138l46.292-92.963c2.082-4.18,0.38-9.258-3.801-11.341
                                c-4.182-2.08-9.26-0.38-11.341,3.801L74.36,283.756V136.058c0-4.672-3.787-8.458-8.458-8.458s-8.458,3.786-8.458,8.458v176.051
                                H8.458c-4.671,0-8.458,3.786-8.458,8.458c0,4.672,3.787,8.458,8.458,8.458h48.987v174.518c0,4.672,3.787,8.458,8.458,8.458
                                s8.458-3.786,8.458-8.458V329.024h226.947l56.94,122.272c1.389,2.984,4.38,4.887,7.667,4.887c0.02,0,0.041,0,0.061,0
                                c3.31-0.025,6.303-1.977,7.657-4.998l25.029-55.826c1.91-4.262,0.004-9.267-4.257-11.178c-4.257-1.908-9.266-0.006-11.178,4.257
                                l-17.457,38.937l-45.801-98.353h89.9l-12.892,28.753c-1.91,4.262-0.004,9.266,4.257,11.178c1.123,0.503,2.298,0.742,3.456,0.742
                                c3.23,0,6.315-1.861,7.722-5l15.995-35.674h54.899l-8.052,8.153c-0.205,0.207-0.397,0.423-0.576,0.646
                                c-0.538,0.668-0.959,1.398-1.263,2.164c-0.509,1.275-0.692,2.65-0.555,3.997c0.055,0.539,0.161,1.074,0.32,1.596
                                c0.396,1.305,1.113,2.533,2.152,3.559c0.413,0.407,0.855,0.763,1.322,1.068c1.399,0.915,3.01,1.371,4.62,1.371
                                c0.546,0,1.091-0.053,1.628-0.158c0.268-0.052,0.536-0.118,0.798-0.196c0.527-0.157,1.041-0.367,1.536-0.629
                                c0.247-0.131,0.489-0.275,0.724-0.432c0.471-0.315,0.918-0.681,1.332-1.101l22.271-22.554
                                C512.813,323.215,512.813,317.919,509.561,314.625z"/>
                        </g>
                    </g>
                    </svg>`,
                    content:        
                    {
                        type: 'lineChart',
                        tagName: "canvas",
                        droppable: false,
                        copyable: false,
                        attributes: {
                            name: 'line-chart',
                            firstCreated: true,
                        },
                    }
            });
    
            editor.BlockManager.add('gaugeChart', {
                category : 'Chart JS',
                label: 'Gauge Chart',
                media: `<svg fill="currentColor" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 1000 1000" enable-background="new 0 0 1000 1000" xml:space="preserve">
                    <g><g transform="translate(0.000000,511.000000) scale(0.100000,-0.100000)"><path d="M4520.4,3747.8c-627.1-74.8-1145-224.4-1678.1-485.2c-498.6-245.5-914.8-540.8-1304.1-932.1C771,1565.3,303.1,631.3,134.3-473.3c-46-303-46-1049.1,1.9-1357.8c71-467.9,216.7-958.9,408.5-1369.4c88.2-193.7,136.2-257,220.6-301.1c78.6-40.3,8390.6-40.3,8469.2,0c84.4,44.1,132.3,107.4,220.6,301.1c193.7,414.3,343.3,920.6,410.4,1388.5c46,316.4,46,1024.1,0,1332.9c-163,1097-638.6,2048.3-1403.9,2809.6C7730.9,3057.4,6841,3515.8,5792,3703.7c-184.1,34.5-318.4,42.2-715.4,46C4808.1,3753.6,4556.9,3751.7,4520.4,3747.8z M4693,2815.8c0-383.6,21.1-456.4,141.9-533.2c153.4-94,364.4-34.5,435.4,122.7c30.7,67.1,36.4,128.5,36.4,410.4v331.8l101.6-13.4c514-59.5,960.9-178.4,1361.7-362.5l107.4-49.9L6750.9,2507c-203.3-347.1-222.5-393.2-211-489.1c24.9-220.6,278.1-347.1,456.4-228.2c71,46,109.3,103.6,293.4,420l118.9,205.2l161.1-120.8c216.7-161.1,427.7-352.9,606-548.5c147.7-163,372.1-452.6,372.1-475.6c0-7.7-115.1-78.6-255.1-157.3c-303-172.6-370.1-228.2-397-328c-44.1-164.9,55.6-337.5,218.6-379.7c113.1-28.8,161.1-11.5,473.7,170.7c145.8,84.4,268.5,153.4,274.3,153.4c17.2,0,182.2-416.2,239.7-604.1c94-308.8,174.5-723,174.5-901.4v-67.1h-341.4c-295.4,0-349.1-3.8-406.6-36.4c-78.6-40.3-153.4-170.7-153.4-264.7c0-94,65.2-205.2,147.7-257c69-42.2,95.9-46,416.2-51.8l343.3-7.7l-13.4-145.8c-28.8-331.8-174.5-893.7-316.4-1217.8l-38.4-90.1l-3918.2,3.8l-3916.2,5.8l-78.6,201.4c-136.2,347.1-239.7,769.1-270.4,1097L717.3-1461l343.3,7.7c322.2,5.8,347.1,7.7,418.1,51.8c80.6,49.9,143.8,164.9,145.8,262.7c0,88.2-76.7,218.6-153.4,258.9c-57.5,32.6-111.2,36.4-410.4,36.4H717.3l13.4,159.2c19.2,235.9,97.8,598.4,191.8,882.2c71,214.8,186,500.6,211,527.4c5.8,3.8,128.5-63.3,276.2-149.6c314.5-182.2,362.5-199.5,475.6-170.7c163,42.2,262.7,214.8,218.6,379.7c-26.9,99.7-94,155.4-397,328c-140,78.6-255.1,149.6-255.1,157.3c0,23,226.3,314.5,366.3,468c170.7,189.9,400.8,398.9,609.9,556.2c88.2,67.1,163,120.8,164.9,118.9c1.9-3.8,71-122.7,153.4-264.7c80.6-141.9,166.9-280,189.9-303c174.5-193.7,494.8-88.2,523.6,172.6c11.5,97.8,0,124.7-207.1,479.5l-130.4,224.4l107.4,49.9c404.7,184.1,1031.8,349.1,1430.7,372.1C4691.1,3145.6,4693,3116.9,4693,2815.8z"/><path d="M6739.4,606.4c-166.9-101.7-404.7-276.2-818.9-606c-517.8-414.2-1285-1056.7-1357.8-1139.2c-63.3-72.9-118.9-226.3-118.9-328c0-113.1,55.6-251.2,138.1-345.2c180.3-203.3,456.5-241.6,703.8-95.9c113.2,67.1,1231.3,1779.8,1492.1,2286.1C6942.7,698.5,6936.9,725.3,6739.4,606.4z"/></g></g>
                </svg>`,
                content:        
                    {
                        type: 'gaugeChart',
                        tagName: "canvas",
                        droppable: false,
                        copyable: false,
                        attributes: {
                            name: 'gauge-chart',
                            firstCreated: true,
                        },
                    }
            });

            editor.BlockManager.add('widgetBox', {
                category : 'Special',
                label: 'Widget Box',
                media: `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 48 48"><path d="M31.611 2.253c-.212.147-1.124 1.668-4.312 7.187-2.224 3.85-4.081 7.13-4.125 7.29-.095.336.013.678.292.932.096.087 3.306 1.967 7.134 4.178 7.531 4.349 7.383 4.276 7.925 3.935.19-.12 1.144-1.711 4.233-7.063 2.193-3.8 4.057-7.066 4.142-7.259.148-.333.149-.367.022-.699-.162-.423.677.094-7.722-4.757-4.977-2.874-6.762-3.867-7-3.894-.246-.029-.382.006-.589.15m7.017 5.724c3.271 1.89 5.943 3.469 5.937 3.51-.006.04-1.565 2.765-3.464 6.055-2.661 4.609-3.482 5.972-3.578 5.935-.196-.075-11.793-6.77-11.897-6.868-.071-.067.623-1.328 2.82-5.129 1.603-2.772 3.17-5.483 3.482-6.026.319-.553.608-.969.66-.949.051.02 2.768 1.582 6.04 3.472m-37.092.157C.965 8.415 1 7.832 1 17c0 7.616.01 8.259.136 8.489C1.431 26.028.949 26 10 26s8.569.028 8.864-.511c.126-.23.136-.873.136-8.489 0-7.616-.01-8.259-.136-8.489-.296-.539.189-.511-8.876-.509-7.399.001-8.213.014-8.452.132M16.96 17v7H3.04V10h13.92v7M1.536 28.134C.965 28.415 1 27.832 1 37c0 7.616.01 8.259.136 8.489C1.431 46.028.949 46 10 46s8.569.028 8.864-.511c.126-.23.136-.873.136-8.489 0-7.616-.01-8.259-.136-8.489-.296-.539.189-.511-8.876-.509-7.399.001-8.213.014-8.452.132m20 0C20.965 28.415 21 27.832 21 37c0 7.616.01 8.259.136 8.489.295.539-.187.511 8.864.511s8.569.028 8.864-.511c.126-.23.136-.873.136-8.489 0-7.616-.01-8.259-.136-8.489-.296-.539.189-.511-8.876-.509-7.399.001-8.213.014-8.452.132M16.96 37v7H3.04V30h13.92v7m20 0v7H23.04V30h13.92v7" fill-rule="evenodd"/></svg>`,
                content:        
                    {
                        type: 'widgetBox',
                        tagName: "div",
                        droppable: false,
                        copyable: false,
                        attributes: {
                            name: 'widget-box',
                            firstCreated: true,
                            uniqueId: ''
                        }
                    }
            });

            editor.BlockManager.add('dataTable', {
                category : 'Special',
                label: 'Datatable',
                media: `<svg version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" viewBox="0 0 1000 1000" enable-background="new 0 0 1000 1000" xml:space="preserve">
                    <metadata> Svg Vector Icons : http://www.onlinewebfonts.com/icon </metadata>
                    <g><g transform="matrix(1 0 0 -1 0 1952)"><path d="M990,1141.7v620.7c0,18-14.6,32.7-32.7,32.7H42.7c-18,0-32.7-14.6-32.7-32.7v-620.7c0-18,14.6-32.7,32.7-32.7h914.7C991.7,1109,990,1141.7,990,1141.7L990,1141.7z M271.3,1141.7H42.7v130.7h228.7V1141.7L271.3,1141.7z M271.3,1305H42.7v130.7h228.7V1305z M271.3,1468.3H42.7V1599h228.7V1468.3z M271.3,1631.7H42.7v130.7h228.7V1631.7z M499,1141.7H304v130.7h196L499,1141.7z M499,1305H304v130.7h196L499,1305z M304,1468.3l0,130.7h195l1-130.7H304L304,1468.3z M500,1631.7H304v130.7h196V1631.7z M728.7,1141.7h-195l-2,130.7h197V1141.7L728.7,1141.7z M728.7,1305h-195l-2,130.7h197V1305L728.7,1305z M728.7,1468.3h-196l-1,130.7h197V1468.3L728.7,1468.3z M728.7,1631.7h-196l-1,130.7h197V1631.7L728.7,1631.7z M957.3,1141.7h-196l-1,130.7h197V1141.7L957.3,1141.7z M957.3,1305h-196l-1,130.7h197V1305L957.3,1305z M957.3,1468.3h-197V1599h197V1468.3L957.3,1468.3z M957.3,1631.7h-196l-1,130.7h197V1631.7L957.3,1631.7z M238.7,1403H75.3v-65.3h163.3V1403z M238.7,1239.7H75.3v-65.3h163.3V1239.7z M238.7,1566.3H75.3V1501h163.3V1566.3z M467.3,1729.7H336.7v-65.3h130.7V1729.7z M696,1729.7H565.3v-65.3H696V1729.7z M238.7,1729.7H75.3v-65.3h163.3V1729.7z M924.7,1729.7H794v-65.3h130.7V1729.7z"/></g></g>
                    </svg>`,
                content: {
                    type: 'dataTable',
                    droppable: false,
                    copyable: false,
                    tagName: "div",        
                    attributes: {
                        name: 'table-datatable',
                        firstCreated: true,
                        uniqueId: '',
                        setting: { table: '', columns: {} }
                    },
                    style: { padding: '5px' },
                }
            });
    
            editor.TraitManager.addType('barchart-data', {
                eventCapture: ['input'],
                createInput({ trait }) {
                    const el = document.createElement('div');
                    el.innerHTML = `
                        <select class="chart-data__sample">
                            <option value="default">Default</option>
                            ${barChartWebApiLists.map(opt => `<option value="${opt.url}">${opt.name}</option>`).join('')}
                        </select>
                    `;
                    
                    return el;
                },
                onEvent({ elInput, component, event }) {
                    const inputType = elInput.querySelector('.chart-data__sample');
                    let data = '';
                    data = inputType.value;
                    component.addAttributes({ data });         
                },
                onUpdate({ elInput, component }) {
                    const data = component.getAttributes().data || '';
                    const inputType = elInput.querySelector('.chart-data__sample');
                    inputType.value = data;
                }
            });

            editor.TraitManager.addType('piechart-data', {
                eventCapture: ['input'],
                createInput({ trait }) {
                    const el = document.createElement('div');
                    el.innerHTML = `
                        <select class="chart-data__sample">
                            <option value="default">Default</option>
                            ${pieChartWebApiLists.map(opt => `<option value="${opt.url}">${opt.name}</option>`).join('')}
                        </select>
                    `;
                    
                    return el;
                },
                onEvent({ elInput, component, event }) {
                    const inputType = elInput.querySelector('.chart-data__sample');
                    let data = '';
                    data = inputType.value;
                    component.addAttributes({ data });         
                },
                onUpdate({ elInput, component }) {
                    const data = component.getAttributes().data || '';
                    const inputType = elInput.querySelector('.chart-data__sample');
                    inputType.value = data;
                }
            });

            editor.TraitManager.addType('linechart-data', {
                eventCapture: ['input'],
                createInput({ trait }) {
                    const el = document.createElement('div');
                    el.innerHTML = `
                        <select class="chart-data__sample">
                            <option value="default">Default</option>
                            ${lineChartWebApiLists.map(opt => `<option value="${opt.url}">${opt.name}</option>`).join('')}
                        </select>
                    `;
                    
                    return el;
                },
                onEvent({ elInput, component, event }) {
                    const inputType = elInput.querySelector('.chart-data__sample');
                    let data = '';
                    data = inputType.value;
                    component.addAttributes({ data });         
                },
                onUpdate({ elInput, component }) {
                    const data = component.getAttributes().data || '';
                    const inputType = elInput.querySelector('.chart-data__sample');
                    inputType.value = data;
                }
            });

            editor.TraitManager.addType('gaugechart-data', {
                eventCapture: ['input'],
                createInput({ trait }) {
                    const el = document.createElement('div');
                    el.innerHTML = `
                        <select class="chart-data__sample">
                            <option value="default">Default</option>
                            ${gaugeChartWebApiLists.map(opt => `<option value="${opt.url}">${opt.name}</option>`).join('')}
                        </select>
                    `;
                    
                    return el;
                },
                onEvent({ elInput, component, event }) {
                    const inputType = elInput.querySelector('.chart-data__sample');
                    let data = '';
                    data = inputType.value;
                    component.addAttributes({ data });         
                },
                onUpdate({ elInput, component }) {
                    const data = component.getAttributes().data || '';
                    const inputType = elInput.querySelector('.chart-data__sample');
                    inputType.value = data;
                }
            });

            editor.TraitManager.addType('datatable-setting', {
                eventCapture: ['input', 'click button'],
                createInput({ trait }) {
                    console.log("createinput");
                    const el = document.createElement('div');
                    
                    el.innerHTML = `
                        <select class="datatable-table__sample" id="datatable-table___table_list">
                            <option value="default" style="color: black;">Default</option>
                            ${Object.keys(mssqlTable).map(key => `<option value="${key}" style="color: black;">${key}</option>`).join('')}
                        </select>
                        <input type="hidden" id="tableChange" value="">
                    `;

                    const elCols = document.createElement('div');
                    elCols.id = "datatable-table__cols";
                    elCols.style.cssText = 'padding: 5px 10px; font-weight: lighter; text-align: left;'

                    this.el.append(elCols);
                    
                    return el;
                },
                onEvent({ elInput, component, event }) {
                    const inputType = elInput.querySelector('.datatable-table__sample');
                    let currentTable = component.getAttributes().setting.table;

                    if (event.type === 'click' && event.target.id === 'traitSubmitButton') {
                        console.log('save');

                        if (inputType.value === currentTable && inputType.value !== 'default') {
                            let setting = { table: inputType.value, columns: { } };
                            let index = 0;

                            document.querySelectorAll('input[name="datatable-setting-column-input"]').forEach(function(elem) {
                                setting.columns[_.camelCase(elem.value)] = { displayable: elem.checked, searchable: false, disable: false, fullName: _.startCase(elem.value), index: index };
                                index++;
                            });

                            document.querySelectorAll('input[name="datatable-setting-searchbox-input"]').forEach(function(elem) {
                                let cloneObjVal = { };
                                cloneObjVal.displayable = setting.columns[_.camelCase(elem.value)].displayable;
                                cloneObjVal.searchable = elem.checked;
                                cloneObjVal.disabled = elem.disabled;
                                cloneObjVal.fullName = setting.columns[_.camelCase(elem.value)].fullName;
                                cloneObjVal.index = setting.columns[_.camelCase(elem.value)].index;

                                setting.columns[_.camelCase(elem.value)] = cloneObjVal;
                            }); 
                            
                            component.addAttributes({ setting });
                            console.log(setting);
                        } else if (inputType.value !== currentTable && inputType.value !== 'default') {
                            let setting = { table: inputType.value, columns: { } };
                            let index = 0;

                            document.querySelectorAll('input[name="datatable-setting-column-input"]').forEach(function(elem) {
                                setting.columns[_.camelCase(elem.value)] = { displayable: elem.checked, searchable: false, disable: false, fullName: _.startCase(elem.value), index: index };
                                index++;
                            });

                            document.querySelectorAll('input[name="datatable-setting-searchbox-input"]').forEach(function(elem) {
                                let cloneObjVal = { };
                                cloneObjVal.displayable = setting.columns[_.camelCase(elem.value)].displayable;
                                cloneObjVal.searchable = elem.checked;
                                cloneObjVal.disabled = elem.disabled;
                                cloneObjVal.fullName = setting.columns[_.camelCase(elem.value)].fullName;
                                cloneObjVal.index = setting.columns[_.camelCase(elem.value)].index;

                                setting.columns[_.camelCase(elem.value)] = cloneObjVal;
                            }); 
                            
                            component.addAttributes({ setting });
                            console.log(setting);
                        } else if (inputType.value === 'default') {
                            let setting = '';
                            setting = { table: '', columns: { } };
                            component.addAttributes({ setting });
                            console.log(setting);
                        }
                    }

                    if (event.type === 'input' && event.target.id === 'datatable-table___table_list') {
                        if (event.target.value !== 'default' && event.target.value !== currentTable) {
                            let parent = this.el.querySelector('#datatable-table__cols');
                            
                            while (parent.firstChild) {
                                parent.removeChild(parent.firstChild);
                            }

                            let tableForColumns = document.createElement('table');
                            tableForColumns.className = "mb-2";
                            tableForColumns.style.cssText = 'width: 100%;';
                            let trFirst = document.createElement('tr');
    
                            let thName = document.createElement('th');
                            thName.innerHTML = "Column Name";
                            
                            let thDisplayable = document.createElement('th');
                            thDisplayable.innerHTML = "Displayable";
                            
                            let thSearchable = document.createElement('th');
                            thSearchable.innerHTML = "Searchable";
    
                            trFirst.appendChild(thName);
                            trFirst.appendChild(thDisplayable);
                            trFirst.appendChild(thSearchable);
    
                            tableForColumns.appendChild(trFirst);
                            
                            for (const [key, value] of Object.entries(mssqlTable[inputType.value])) {
                                let trCol = document.createElement('tr');
    
                                let tdName = document.createElement('th');
                                tdName.innerHTML = _.startCase(value['colName']);

                                let tdDisplayable = document.createElement('th');
                                tdDisplayable.style.cssText = 'text-align: center;';

                                let inputDisplayableTrueFalse = document.createElement('input');
                                inputDisplayableTrueFalse.name = "datatable-setting-column-input";
                                inputDisplayableTrueFalse.type = 'checkbox';
                                inputDisplayableTrueFalse.value = _.camelCase(value['colName']);
                                inputDisplayableTrueFalse.checked = true;
                                inputDisplayableTrueFalse.onchange = function (event) {
                                    let id = value['colName'] + "-searchbox";
                                    let checkb = document.getElementById(id);
                    
                                    if (!this.checked) {
                                        if (checkb.disabled === false) {
                                            checkb.disabled = true;
                                        }
                                    } else {
                                        if (checkb.disabled === true) {
                                            checkb.disabled = false;
                                        }
                                    }
                                };
    
                                tdDisplayable.append(inputDisplayableTrueFalse);
        
                                let tdSearchable = document.createElement('th');
                                tdSearchable.style.cssText = 'text-align: center;';
                                
                                let inputSearchableTrueFalse = document.createElement('input');
                                inputSearchableTrueFalse.id = value['colName'] + "-searchbox";
                                inputSearchableTrueFalse.name = "datatable-setting-searchbox-input";
                                inputSearchableTrueFalse.type = 'checkbox';
                                inputSearchableTrueFalse.checked = true;
                                inputSearchableTrueFalse.disabled = false;
                                inputSearchableTrueFalse.value = _.camelCase(value['colName']);

                                tdSearchable.append(inputSearchableTrueFalse);
                                
                                trCol.appendChild(tdName);
                                trCol.appendChild(tdDisplayable);
                                trCol.appendChild(tdSearchable);
    
                                tableForColumns.appendChild(trCol);
                            }
                            
                            parent.appendChild(tableForColumns);
    
                            let createSubmitButton = document.createElement('button');
                            createSubmitButton.className = "btn btn-sm btn-primary";
                            createSubmitButton.type = 'submit';
                            createSubmitButton.id = 'traitSubmitButton';
                            createSubmitButton.innerHTML = 'Submit';
                            createSubmitButton.onclick = function() {
    
                            };
                            
                            parent.appendChild(createSubmitButton);
                        } else if (event.target.value !== 'default' && event.target.value === currentTable) {
                            let parent = this.el.querySelector('#datatable-table__cols');
                  
                            while (parent.firstChild) {
                                parent.removeChild(parent.firstChild);
                            }
    
                            let tableForColumns = document.createElement('table');
                            tableForColumns.className = "mb-2";
                            tableForColumns.style.cssText = 'width: 100%;';
                            let trFirst = document.createElement('tr');
    
                            let thName = document.createElement('th');
                            thName.innerHTML = "Column Name";

                            let thDisplayable = document.createElement('th');
                            thDisplayable.innerHTML = "Displayable";
    
                            let thSearchable = document.createElement('th');
                            thSearchable.innerHTML = "Searchable";
    
                            trFirst.appendChild(thName);
                            trFirst.appendChild(thDisplayable);
                            trFirst.appendChild(thSearchable);
    
                            tableForColumns.appendChild(trFirst);
                            
                            for (const [key, value] of Object.entries(component.getAttributes().setting.columns)) {
                                let trCol = document.createElement('tr');
    
                                let tdName = document.createElement('th');
                                tdName.innerHTML = _.startCase(key);
    
                                let tdDisplayable = document.createElement('th');
                                tdDisplayable.style.cssText = 'text-align: center;';
                                
                                let inputDisplayableTrueFalse = document.createElement('input');
                                inputDisplayableTrueFalse.name = "datatable-setting-column-input";
                                inputDisplayableTrueFalse.type = 'checkbox';
                                inputDisplayableTrueFalse.value = key;
                                inputDisplayableTrueFalse.checked = value['displayable'];
                                inputDisplayableTrueFalse.onchange = function (event) {
                                    let id = key + "-searchbox";
                                    let checkb = document.getElementById(id);
                    
                                    if (!this.checked) {
                                        if (checkb.disabled === false) {
                                            checkb.disabled = true;
                                        }
                                    } else {
                                        if (checkb.disabled === true) {
                                            checkb.disabled = false;
                                        }
                                    }
                                };    
    
                                tdDisplayable.append(inputDisplayableTrueFalse);
        
                                let tdSearchable = document.createElement('th');
                                tdSearchable.style.cssText = 'text-align: center;';
                                
                                let inputSearchableTrueFalse = document.createElement('input');
                                inputSearchableTrueFalse.id = key + "-searchbox";
                                inputSearchableTrueFalse.name = "datatable-setting-searchbox-input";
                                inputSearchableTrueFalse.type = 'checkbox';
                                inputSearchableTrueFalse.value = key;
                                inputSearchableTrueFalse.checked = value['searchable'];
                                inputSearchableTrueFalse.disabled = value['disabled'];
    
                                tdSearchable.append(inputSearchableTrueFalse);
                                
                                trCol.appendChild(tdName);
                                trCol.appendChild(tdDisplayable);
                                trCol.appendChild(tdSearchable);
    
                                tableForColumns.appendChild(trCol);
                            }
                            
                            parent.appendChild(tableForColumns);
    
                            let createSubmitButton = document.createElement('button');
                            createSubmitButton.className = "btn btn-sm btn-primary";
                            createSubmitButton.type = 'submit';
                            createSubmitButton.id = 'traitSubmitButton';
                            createSubmitButton.innerHTML = 'Submit';
                            createSubmitButton.onclick = function() {
    
                            };
                            
                            parent.appendChild(createSubmitButton);   
                        } else if (event.target.value === 'default') {
                            let parent = this.el.querySelector('#datatable-table__cols');
                  
                            while (parent.firstChild) {
                                parent.removeChild(parent.firstChild);
                            }
                            
                            let createSubmitButton = document.createElement('button');
                            createSubmitButton.className = "btn btn-sm btn-primary";
                            createSubmitButton.type = 'submit';
                            createSubmitButton.id = 'traitSubmitButton';
                            createSubmitButton.innerHTML = 'Submit';
                            createSubmitButton.onclick = function() {
    
                            };
                            
                            parent.appendChild(createSubmitButton);
                        }
                    }
                    // let attrs = component.getAttributes();
                    // attrs.setting.table = inputType.value;

                    // if (attrs.setting.table !== 'default') {

                    //     let child = document.createElement('div');
                    //     this.el.querySelector('#datatable-table__cols').append(child);
                    //     console.log(this.el.querySelector('#datatable-table__cols'));
                    //     console.log(this.el.querySelector('#datatable-table__cols').children.length);
                    //     if (this.el.querySelector('#datatable-table__cols').children.length > 0) {
                    //         this.el.querySelector('#datatable-table__cols').empty();
                    //         this.el.querySelector('#datatable-table__cols').append("<h1>sadadasdasd</h1>");
                    //     } else {
                    //         this.el.querySelector('#datatable-table__cols').append("<h1>sadadasdasd</h1>");
                    //     }
                    // }

                    // component.setAttributes(attrs);
                },
                onUpdate({ elInput, component }) {
                    // console.log(component.getAttributes());
                    console.log("onupdate", component.getAttributes());
                    const table = component.getAttributes().setting.table || '';
                    const inputType = elInput.querySelector('.datatable-table__sample');
                    inputType.value = table;

                    if (table !== 'default') {
                        let parent = this.el.querySelector('#datatable-table__cols');
                  
                        if (parent === null) {
                            const elCols = document.createElement('div');
                            elCols.id = "datatable-table__cols";
                            elCols.style.cssText = 'padding: 5px 10px; font-weight: lighter; text-align: left;'
        
                            this.el.append(elCols);

                            parent = this.el.querySelector('#datatable-table__cols');
                        }

                        while (parent.firstChild) {
                            parent.removeChild(parent.firstChild);
                        }

                        let tableForColumns = document.createElement('table');
                        tableForColumns.className = "mb-2";
                        tableForColumns.style.cssText = 'width: 100%;';
                        let trFirst = document.createElement('tr');

                        let thName = document.createElement('th');
                        thName.innerHTML = "Column Name";

                        let thDisplayable = document.createElement('th');
                        thDisplayable.innerHTML = "Displayable";

                        let thSearchable = document.createElement('th');
                        thSearchable.innerHTML = "Searchable";

                        trFirst.appendChild(thName);
                        trFirst.appendChild(thDisplayable);
                        trFirst.appendChild(thSearchable);

                        tableForColumns.appendChild(trFirst);
                        
                        for (const [key, value] of Object.entries(component.getAttributes().setting.columns)) {
                            let trCol = document.createElement('tr');

                            let tdName = document.createElement('th');
                            tdName.innerHTML = _.startCase(key);

                            let tdDisplayable = document.createElement('th');
                            tdDisplayable.style.cssText = 'text-align: center;';
                            
                            let inputDisplayableTrueFalse = document.createElement('input');
                            inputDisplayableTrueFalse.name = "datatable-setting-column-input";
                            inputDisplayableTrueFalse.type = 'checkbox';
                            inputDisplayableTrueFalse.value = key;
                            inputDisplayableTrueFalse.checked = value['displayable'];
                            inputDisplayableTrueFalse.onchange = function (event) {
                                let id = key + "-searchbox";
                                let checkb = document.getElementById(id);
                    
                                if (!this.checked) {
                                    if (checkb.disabled === false) {
                                        checkb.disabled = true;
                                    }
                                } else {
                                    if (checkb.disabled === true) {
                                        checkb.disabled = false;
                                    }
                                }
                            };

                            tdDisplayable.append(inputDisplayableTrueFalse);
    
                            let tdSearchable = document.createElement('th');
                            tdSearchable.style.cssText = 'text-align: center;';
                            
                            let inputSearchableTrueFalse = document.createElement('input');
                            inputSearchableTrueFalse.id = key + "-searchbox";
                            inputSearchableTrueFalse.name = "datatable-setting-searchbox-input";
                            inputSearchableTrueFalse.type = 'checkbox';
                            inputSearchableTrueFalse.value = key;
                            inputSearchableTrueFalse.checked = value['searchable'];
                            inputSearchableTrueFalse.disabled = value['disabled'];

                            tdSearchable.append(inputSearchableTrueFalse);
                            
                            trCol.appendChild(tdName);
                            trCol.appendChild(tdDisplayable);
                            trCol.appendChild(tdSearchable);

                            tableForColumns.appendChild(trCol);
                        }
                        
                        parent.appendChild(tableForColumns);

                        let createSubmitButton = document.createElement('button');
                        createSubmitButton.className = "btn btn-sm btn-primary";
                        createSubmitButton.type = 'submit';
                        createSubmitButton.id = 'traitSubmitButton';
                        createSubmitButton.innerHTML = 'Submit';
                        createSubmitButton.onclick = function() {

                        };
                        
                        parent.appendChild(createSubmitButton);

                        // let parent = this.el.querySelector('#datatable-table__cols');

                        // if (parent.children.length > 0) {
                        //     let child = parent.firstElementChild;
                        //     parent.removeChild(child);
                        // } 

                        // let labelForSelectedColumns = document.createElement('label');
                        // labelForSelectedColumns.innerHTML = "Columns";

                        // let container = document.createElement('div');
                        // container.style.cssText = 'display: flex; justify-content: flex-start; align-items: center;';
                        
                        // let labelWrapper = document.createElement('div');
                        // labelWrapper.className = 'gjs-label-wrp';

                        // let label = document.createElement('div');
                        // label.className = '.gjs-label';
                        // label.innerHTML = 'Test';

                        // labelWrapper.appendChild(label);
                        // container.appendChild(labelWrapper);

                        // let fieldWrapper = document.createElement('div');

                        // let input = document.createElement('input');
                        // input.type = 'checkbox';
                        // input.value = '';
                        // input.checked = true;

                        // fieldWrapper.appendChild(input);

                        // container.appendChild(fieldWrapper);

                        // parent.appendChild(labelForSelectedColumns);
                        // parent.appendChild(container);
                    }
                }
            });
    
            editor.DomComponents.addType('barChart', {
                isComponent: el => {
                    // console.log(el);
                    return el.tagName === 'CANVAS';
                },
                model: {
                    defaults: {
                        components: model => {
                            if (model.getAttributes().firstCreated) {
                                let attrs = model.getAttributes();
                                attrs.id = generateRandomId();
                                attrs.firstCreated = false;
    
                                model.setAttributes(attrs);
                            }
                        },
                        traits: [
                            'name',
                            {
                                type: 'barchart-data',
                                name: 'chartData',
                                label: 'Chart Data',
                            },
                        ]
                    },
                    init() {
                        if (this.get('script') === '') {
                            this.addAttributes({ data: 'default' });
                            this.set('script', generateDefaultScript(this.getId(), "bar"));
                        }
                        this.on('change:attributes:data', this.handleAttrChange);            
                    },
                    handleAttrChange() {
                        console.log('Attributes updated: ', this.getAttributes().data);
                        const datac = this.getAttributes().data;
                        const getId = this.getAttributes().id;
    
                        if (datac === 'default') {
                            let dataScript = '[900, 300, 600, 659, 150]';
                            this.set('script', generateDefaultScript(getId, 'bar', dataScript));
                        } else {
                            this.set('script', generateScript(getId, datac , 'bar'));        
                        }
                    },
                },
                view: {
    
                }
            });

            editor.DomComponents.addType('pieChart', {
                model: {
                    defaults: {
                        components: model => {
                            if (model.getAttributes().firstCreated) {
                                let attrs = model.getAttributes();
                                attrs.id = generateRandomId();
                                attrs.firstCreated = false;
    
                                model.setAttributes(attrs);
                            }
                        },
                        traits: [
                            'name',
                            {
                                type: 'piechart-data',
                                name: 'chartData',
                                label: 'Chart Data',
                            },
                        ]
                    },
                    init() {
                        if (this.get('script') === '') {
                            this.addAttributes({ data: 'default' });
                            this.set('script', generateDefaultScript(this.getId(), "pie"));
                        }
                        this.on('change:attributes:data', this.handleAttrChange);            
                    },
                    handleAttrChange() {
                        console.log('Attributes updated: ', this.getAttributes().data);
                        const datac = this.getAttributes().data;
                        const getId = this.getAttributes().id;
    
                        if (datac === 'default') {
                            let dataScript = '[900, 300, 600, 659, 150]';
                            this.set('script', generateDefaultScript(getId, 'pie', dataScript));
                        } else {
                            this.set('script', generateScript(getId, datac, 'pie'));        
                        }
                    },
                },
                view: {
    
                }
            });

            editor.DomComponents.addType('lineChart', {
                model: {
                    defaults: {
                        components: model => {
                            if (model.getAttributes().firstCreated) {
                                let attrs = model.getAttributes();
                                attrs.id = generateRandomId();
                                attrs.firstCreated = false;
    
                                model.setAttributes(attrs);
                            }
                        },
                        traits: [
                            'name',
                            {
                                type: 'linechart-data',
                                name: 'chartData',
                                label: 'Chart Data',
                            },
                        ]
                    },
                    init() {
                        if (this.get('script') === '') {
                            this.addAttributes({ data: 'default' });
                            this.set('script', generateDefaultScript(this.getId(), "line"));
                        }
                        this.on('change:attributes:data', this.handleAttrChange);            
                    },
                    handleAttrChange() {
                        console.log('Attributes updated: ', this.getAttributes().data);
                        const datac = this.getAttributes().data;
                        const getId = this.getAttributes().id;
    
                        if (datac === 'default') {
                            let dataScript = '[900, 300, 600, 659, 150]';
                            this.set('script', generateDefaultScript(getId, 'line', dataScript));
                        } else {
                            this.set('script', generateScript(getId, datac, 'line'));        
                        }
                    },
                },
                view: {
    
                }
            });

            editor.DomComponents.addType('gaugeChart', {
                model: {
                    defaults: {
                        components: model => {
                            if (model.getAttributes().firstCreated) {
                                let attrs = model.getAttributes();
                                attrs.id = generateRandomId();
                                attrs.firstCreated = false;
    
                                model.setAttributes(attrs);
                            }
                        },
                        traits: [
                            'name',
                            {
                                type: 'gaugechart-data',
                                name: 'chartData',
                                label: 'Chart Data',
                            },
                        ]
                    },
                    init() {
                        if (this.get('script') === '') {
                            this.addAttributes({ data: 'default' });
                            this.set('script', generateDefaultScript(this.getId(), "doughnut"));
                        }
                        this.on('change:attributes:data', this.handleAttrChange);            
                    },
                    handleAttrChange() {
                        console.log('Attributes updated: ', this.getAttributes().data);
                        const datac = this.getAttributes().data;
                        const getId = this.getAttributes().id;
    
                        if (datac === 'default') {
                            let dataScript = '[900, 300, 600, 659, 150]';
                            this.set('script', generateDefaultScript(getId, 'doughnut', dataScript));
                        } else {
                            this.set('script', generateScript(getId, datac, 'doughnut'));        
                        }
                    },
                },
                view: {
    
                }
            });

            editor.DomComponents.addType('widgetBox', {
                isComponent: el => {
                    return el.tagName === 'div';
                },
                model: {
                    defaults: {
                        components: model => {

                        },
                        traits: [

                        ]
                    },
                    init() {
                        if (this.getAttributes().firstCreated) {
                            const container = document.createElement('div');
                            let widgetBoxIcons = "";

                            Object.keys(widgetBoxIconLists).forEach(function(key, index) {
                                widgetBoxIcons += '<option value="' + key + '">' + key + '</option>';
                            });
                            
                            const inputHtml =  `
                                <div class="form-group">
                                    <label for="widgetTitle">Widget Title</label>
                                    <input type="text" class="form-control" id="widgetTitle" placeholder="Enter Widget Title">
                                </div>
                                <br />
                                <div class="form-group">
                                    <label for="widgetIcon">Widget Icon</label>
                                    <select class="form-select" id="widgetIcon">
                                        <option selected>Default</option>
                                        ${widgetBoxIcons}
                                    </select>
                                </div>
                                <br />
                                <div class="form-group">
                                    <label for="widgetRow">Widget Row (Description)</label>
                                    <select class="form-select" id="widgetRow">
                                        <option selected>Default</option>
                                        ${widgetBoxDescriptionLists.map(x => `<option value="${x}">${x}</option>`).join('')}
                                    </select>
                                </div>
                                <br />
                                <div class="form-group">
                                    <label for="widgetColumn">Widget Column (Table)</label>
                                    <select class="form-select" id="widgetColumn">
                                        <option selected>Default</option>
                                        ${widgetBoxColumnLists.map(x => `<option value="${x}">${x}</option>`).join('')}
                                    </select>
                                </div>
                                <br />
                            `;

                            const btnSubmit = document.createElement('button');
                            btnSubmit.type = "submit";
                            btnSubmit.innerHTML = "Submit";
                            btnSubmit.classList.add("btn", "btn-primary");

                            btnSubmit.onclick = function() {                                
                                editor.Modal.close();
                            };
                            
                            container.innerHTML = inputHtml;
                            container.appendChild(btnSubmit);

                            editor.Modal.open({
                                title: 'Widget Box Setting',
                                content: container,
                            });

                            editor.Modal.onceClose(() => {
                                const widgetTitle = document.getElementById("widgetTitle").value;
                                const widgetIcon = document.getElementById("widgetIcon").value;
                                const widgetRow = document.getElementById("widgetRow").value;
                                const widgetColumn = document.getElementById("widgetColumn").value;

                                let innerIconHTML = "";
                                let widgetTitleHTML = "No Title";
                                let widgetRealTimeValue = "<span>No Value</span>";
                                let setJSScript = false;
                                var uniqueId = "";

                                if (widgetTitle !== "") {
                                    widgetTitleHTML = widgetTitle;
                                }
                                if (widgetRow !== "Default" && widgetColumn !== "Default") {
                                    uniqueId = generateRandomId();
                                    widgetRealTimeValue = `<span id="${uniqueId}">Loading..</span>`;
                                    setJSScript = true;
                                }

                                if (widgetIcon !== "Default") {
                                    innerIconHTML = widgetBoxIconLists[widgetIcon];
                                } else {
                                    innerIconHTML = widgetBoxIconLists["Multi User"];
                                }

                                this.set('content', `<div class="card"><div class="card-body"><div class="d-flex justify-content-between"><div><p class="fw-medium mb-0 text-muted">${widgetTitleHTML}</p><h2 class="mt-4 ff-secondary fw-semibold">${widgetRealTimeValue}</h2></div><div><div class="avatar-sm flex-shrink-0"><span class="avatar-title rounded-circle fs-2 shadow bg-soft-info">${innerIconHTML}</span></div></div></div></div></div>`);

                                if (setJSScript) {
                                    let baseURL = axios.defaults.baseURL + "/WidgetBoxes/GetWidgetBoxRealTime"
                                    
                                    const innerScript = `
                                        function Test() {
                                            let randomNbr = randomIntFromInterval(100, 500);
                                            document.getElementById("${uniqueId}").innerHTML = randomNbr + " K";
                                            /* let cookie = JSON.parse(sessionStorage.getItem("authUser"));
                                            $.ajax({
                                                type: "GET",
                                                url: "${baseURL}",
                                                data: {
                                                    description: '${widgetRow}',
                                                    column: '${widgetColumn}'
                                                },
                                                dataType: "json",
                                                beforeSend: function (xhr) {
                                                    xhr.setRequestHeader ("Authorization", "Bearer " + cookie.token);
                                                },
                                                success: function(resp) {
                                                    console.log(resp);
                                                    document.getElementById("${uniqueId}").innerHTML = resp;
                                                },
                                                error: function(xhr) {
                            
                                                },
                                                complete: function() {
                            
                                                },
                                            }); */
                                        };

                                        Test();
                                    `;

                                    this.set('script', innerScript);

                                    var iframe = document.getElementsByTagName('iframe')[0];
                                    var oldScript = iframe.contentWindow.document.getElementById("intervalScripting");

                                    let script2 = `
                                        widgetBoxList.push('${uniqueId}');
                                        setIntervalF();
                                    `;

                                    const newScript = document.createElement('script');
                                    newScript.id = 'intervalScripting';
                                    newScript.type = 'text/javascript';
                                    newScript.innerHTML = script2;

                                    oldScript.parentNode.replaceChild(newScript, oldScript);
                                    iframe.contentWindow.document.body.appendChild(newScript);

                                    let attrs = this.getAttributes();
                                    attrs.uniqueId = uniqueId;
                                    this.setAttributes(attrs);
                                }
                            });

                            let attrs = this.getAttributes();
                            attrs.firstCreated = false;
                            this.setAttributes(attrs);
                        }
                    },
                    removed() {
                        var iframe = document.getElementsByTagName('iframe')[0];
                        var oldScript = iframe.contentWindow.document.getElementById("intervalScripting");
     
                        let attrs = this.getAttributes();
                        let uniqueId = attrs.uniqueId;

                        if (uniqueId !== '') {
                            let script2 = `
                                widgetBoxList = widgetBoxList.filter(function(x) {
                                    return x !== '${uniqueId}';
                                });
                                setIntervalF();
                            `;

                            const newScript = document.createElement('script');
                            newScript.id = 'intervalScripting';
                            newScript.type = 'text/javascript';
                            newScript.innerHTML = script2;

                            oldScript.parentNode.replaceChild(newScript, oldScript);
                            iframe.contentWindow.document.body.appendChild(newScript);
                        }
                    }
                },
                view: {
                    // events: {
                    //     click: 'innerElClick',
                    // },
                    // innerElClick(ev) {
                    //     console.log("innerElClick");
                    // },     
                    // init({ model }) {
                    //     console.log('Local hook: view.init');
                    // },
                    // removed(test) {
                    //     console.log(test);
                    // },
                    // onRender({ el }) {
                    //     console.log('Local hook: view.onrender');
                    // },
                }
            });

            editor.DomComponents.addType('dataTable', {
                isComponent: el => {
                    return el.tagName === 'div';
                },
                model: {
                    defaults: {
                        components: model => {

                        },
                        traits: [
                            {
                                type: 'datatable-setting',
                                name: 'datatableSetting',
                                label: 'Table',
                            }                        
                        ]
                    },
                    init() {
                        if (this.getAttributes().firstCreated) {
                            const container = document.createElement('div');
                            let tableListsHTML = "";
                            let tableColumnListsHTML = "";

                            Object.keys(mssqlTable).forEach(function(key, index) {
                                tableListsHTML += '<option value="' + key + '">' + key + '</option>';
                                // mssqlTable[key].map((v, index) => {
                                //     console.log(v);
                                // });
                            });

                            let hehe = `<div class="form-group">
                                <label for="selectedColumns">Columns</label>
                                <br />
                                <div class="form-check form-check-inline">
                                    <input class="form-check-input" type="checkbox" name="datatable-setting-column-input" onchange="console.log(checked, this)" value="" checked>
                                    <label class="form-check-label">Column 1</label>
                                </div>
                                <div class="form-check form-check-inline">
                                    <input class="form-check-input" type="checkbox" name="datatable-setting-searchbox-input" value="" checked>
                                    <label class="form-check-label">Column 1 (Search Box)</label>
                                </div>
                                <br />
                                <div class="form-check form-check-inline">
                                    <input class="form-check-input" type="checkbox" name="datatable-setting-column-input" onchange="console.log(checked, this)" value="" checked>
                                    <label class="form-check-label">Column 2</label>
                                </div>
                                <div class="form-check form-check-inline">
                                    <input class="form-check-input" type="checkbox" name="datatable-setting-searchbox-input" value="" checked>
                                    <label class="form-check-label">Column 2 (Search Box)</label>
                                </div>                         
                            </div>`;
                            
                            const inputHtml =  `
                                <div class="form-group">
                                    <label for="selectedTable">Table</label>
                                    <select class="form-select" id="mssqlTable" onchange="OnSelectTable(value, ${JSON.stringify(mssqlTable).replaceAll('"','\'')})">
                                        <option value="default" selected>Default</option>
                                        ${tableListsHTML}
                                    </select>
                                </div>
                                <br />
                                <div id="showColumns" style="display: none;">
                 
                                </div>    
                            `;

                            const btnSubmit = document.createElement('button');
                            btnSubmit.type = "submit";
                            btnSubmit.innerHTML = "Submit";
                            btnSubmit.classList.add("btn", "btn-primary");

                            btnSubmit.onclick = function() {                                
                                editor.Modal.close();
                            };
                            
                            container.innerHTML = inputHtml;
                            container.appendChild(btnSubmit);

                            editor.Modal.open({
                                title: 'DataTable Box Setting',
                                content: container,
                            });

                            editor.Modal.onceClose(() => {
                                let selectedTable = document.getElementById("mssqlTable");
                                
                                if (selectedTable.value !== "default") {
                                    let obj = { table: selectedTable.value, columns: { } };
                                    let index = 0;

                                    document.querySelectorAll('input.form-check-input[name="datatable-setting-column-input"]').forEach(function(elem) {
                                        obj.columns[_.camelCase(elem.value)] = { displayable: elem.checked, searchable: false, disable: false, fullName: _.startCase(elem.value), index: index };
                                        index++;
                                        
                                        // remove reference of element, to avoid duplicated record
                                        elem.remove();
                                    });

                                    document.querySelectorAll('input.form-check-input[name="datatable-setting-searchbox-input"]').forEach(function(elem) {
                                        let cloneObjVal = { };
                                        cloneObjVal.displayable = obj.columns[_.camelCase(elem.value)].displayable;
                                        cloneObjVal.searchable = elem.checked;
                                        cloneObjVal.disabled = elem.disabled;
                                        cloneObjVal.fullName = obj.columns[_.camelCase(elem.value)].fullName;
                                        cloneObjVal.index = obj.columns[_.camelCase(elem.value)].index;

                                        obj.columns[_.camelCase(elem.value)] = cloneObjVal;
                                        
                                        // remove reference of element, to avoid duplicated record
                                        elem.remove();
                                    });
    
                                    // document.querySelectorAll('input.form-check-input[name="datatable-setting-searchbox-input"]:checked:not(:disabled)').forEach(function(elem) {
                                    //     let cloneObjVal = { };
                                    //     cloneObjVal.displayable = obj.columns[_.camelCase(elem.value)].displayable;
                                    //     cloneObjVal.searchable = true;
                                    //     cloneObjVal.fullName = obj.columns[_.camelCase(elem.value)].fullName;
                                    //     cloneObjVal.index = obj.columns[_.camelCase(elem.value)].index;

                                    //     obj.columns[_.camelCase(elem.value)] = cloneObjVal;
                                        
                                    //     // remove reference of element, to avoid duplicated record
                                    //     elem.remove();
                                    // });

                                    if (Object.keys(obj.columns).length > 0) {
                                        let attrs = this.getAttributes();
                                        attrs.firstCreated = false;
            
                                        const uniqueId = generateRandomId();
                                        attrs.uniqueId = uniqueId; 
                                        attrs.setting = { table: obj.table, columns: obj.columns };

                                        // const innerHTML = generateDataTableHTML('notdefault', uniqueId, obj);
            
                                        // this.set('content', innerHTML);
                                        // this.set('script', 
                                        //     generateDataTableScript('notdefault', uniqueId, obj)
                                        // );
            
                                        this.setAttributes(attrs);
                                    }
                                    else {
                                        let attrs = this.getAttributes();
                                        attrs.firstCreated = false;
            
                                        const uniqueId = generateRandomId();
                                        attrs.uniqueId = uniqueId; 
                                        attrs.setting = { table: 'default', columns: { } };

                                        // const innerHTML = generateDataTableHTML('default', uniqueId);
            
                                        // this.set('content', innerHTML);
                                        // this.set('script', 
                                        //     generateDataTableScript('default', uniqueId)
                                        // );
            
                                        this.setAttributes(attrs);                         
                                    }
                                } else {    
                                    let attrs = this.getAttributes();
                                    attrs.firstCreated = false;
        
                                    const uniqueId = generateRandomId();
                                    attrs.uniqueId = uniqueId; 
                                    attrs.setting = { table: 'default', columns: { } };

                                    // const innerHTML = generateDataTableHTML('default', uniqueId);
        
                                    // this.set('content', innerHTML);
                                    // this.set('script', 
                                    //     generateDataTableScript('default', uniqueId)
                                    // );
        
                                    this.setAttributes(attrs);
                                }
                            });
                        }

                        this.on('change:attributes:setting', this.handleAttrChange);
                    },
                    handleAttrChange() {
                        console.log("onchange", this.getAttributes());
                        let attrs = this.getAttributes();
                        let table = attrs.setting.table;
                        let columns  = attrs.setting.columns;
                        let uniqueId = attrs.uniqueId;

                        let obj = { table, columns };
                        let displayableColumns = Object.entries(obj.columns)
                            .filter(([key, value]) => value['displayable'] === true)

                        if (table !== 'default') {
                            if (displayableColumns.length > 0) { 
                                const innerHTML = generateDataTableHTML('notdefault', uniqueId, obj);
            
                                this.set('content', innerHTML);
                                this.set('script', 
                                    generateDataTableScript('notdefault', uniqueId, obj)
                                );
                            } else {
                                const innerHTML = generateDataTableHTML('default', uniqueId);
            
                                this.set('content', innerHTML);
                                this.set('script', 
                                    generateDataTableScript('default', uniqueId)
                                );                                
                            }
                        } else {
                            const innerHTML = generateDataTableHTML('default', uniqueId);
        
                            this.set('content', innerHTML);
                            this.set('script', 
                                generateDataTableScript('default', uniqueId)
                            );                    
                        }
                    },
                },
                view: {
                    
                }
            });
    
            editor.Panels.addButton('options', [ 
                { 
                    id: 'save', 
                    className: 'fa fa-floppy-o', 
                    command: function(editorCopy, sender) 
                        { 
                            const componentData = JSON.stringify(editor.getComponents());
                            const projectData = JSON.stringify(editor.getProjectData());
                            const componentssData = JSON.stringify(componentsData);
                            axios.put("/DashboardBuilderDatas/PutDashboardBuilderData?id=1", { id: 1, data: componentData, projectData: projectData, componentsData: componentssData }).then((response) => {
                                console.log(response);
                            });
                        }, 
                    attributes: { title: 'Save Template' } 
                },
                { 
                    id: 'back', 
                    className: 'fa fa-home', 
                    command: function(editorCopy, sender) 
                        {
                            navigate("/home");
                        }, 
                    attributes: { title: 'Return To Home Page' } 
                },
            ]);

            editor.DomComponents.addType('wrapper', {
                model: {
                    defaults: {
                        droppable: '[data-gjs-type="containerRow"]',
                    }
                },
                view: {

                }
            });

            editor.DomComponents.addType('containerRow', {
                model: {
                    defaults: {
                        droppable: '[data-gjs-type="containerColumn"]',
                        copyable: false
                    }
                },
                view: {

                }
            });

            editor.DomComponents.addType('containerColumn', {
                model: {
                    defaults: {
                        droppable: ['[data-gjs-type="widgetBox"]', '[data-gjs-type="barChart"]', 
                            '[data-gjs-type="pieChart"]', '[data-gjs-type="gaugeChart"]', '[data-gjs-type="lineChart"]', '[data-gjs-type="text"]', '[data-gjs-type="dataTable"]'],
                        copyable: false,
                        resizable: true
                    }
                },
                view: {

                }
            });

            editor.on('component:add', (model) => {
                const modelS = model;
                const selectComponent = ['barChart', 'pieChart', 'widgetBox', 'gaugeChart', 'lineChart', 'dataTable'];
                
                if (modelS !== undefined) {
                    if (selectComponent.includes(modelS.get('type'))) {
                        const parent = modelS.parent();
                        
                        if (parent.get('type') === 'containerColumn') {
                            if (parent.components().length > 0) {
                                const style = parent.getStyle();
                                
                                if (style['height'] !== undefined) {
                                    delete style['height'];
                                }
    
                                parent.setStyle(style); 
                            }
                        }
                    }
                }
            });

            editor.on('component:remove', (model) => {
                const modelS = model;
                const selectComponent = ['barChart', 'pieChart', 'widgetBox', 'gaugeChart', 'lineChart', 'dataTable'];
                
                if (modelS !== undefined) {
                    if (selectComponent.includes(modelS.get('type'))) {
                        const parent = modelS.parent();
                        
                        if (parent.get('type') === 'containerColumn') {
                            if (parent.components().length === 0) {
                                const style = parent.getStyle();
                        
                                if (style['height'] === undefined) {
                                    style['height'] = '75px';
                                }
                
                                parent.setStyle(style); 
                            }
                        }
                    }
                }
            });

            editor.on('load', (props) => {
                const domComponents = editor.DomComponents;
                let widgetBoxIdLists = [];

                domComponents.getComponents().forEach((component) => {
                    let type = component.get('type');

                    if (type === 'containerRow') {
                        let row = component.get('components');
                        if (row.length > 0) {
                            let columns = row.models;

                            columns.forEach((innerComponent) => {
                                let isTypeCol = innerComponent.get('type');

                                if (isTypeCol === 'containerColumn') {
                                    let innerElement = innerComponent.get('components');

                                    if (innerElement.length > 0) {
                                        let innerChild = innerElement.models;
                                        
                                        if (innerChild.length === 1) {
                                            if (innerChild[0].get('type') === 'widgetBox') {
                                                if (innerChild[0].getAttributes()['uniqueId'] !== '') {
                                                    widgetBoxIdLists.push("'" + innerChild[0].getAttributes()['uniqueId'] + "'");
                                                }
                                            }
                                        }
                                    }
                                }
                            });
                        }
                    }
                });
                
                var iframe = document.getElementsByTagName('iframe')[0];
                var script = document.createElement('script');

                if (widgetBoxIdLists.length > 0) {
                    script.id = 'intervalScripting';
                    script.type = 'text/javascript';
                    script.innerHTML = `
                        console.log(intervalVar);
                        widgetBoxList.push(${widgetBoxIdLists.join(', ')});
                        setIntervalF();
                        console.log(widgetBoxList);
                    `;    
                }
                else {
                    script.id = 'intervalScripting';
                    script.type = 'text/javascript';
                    script.innerHTML = `
                        console.log(intervalVar);
                        console.log(widgetBoxList);
                    `;    
                }

                iframe.contentWindow.document.body.appendChild(script);
            });

            editor.loadProjectData(projectData);
        }
    }, [navigate, apiLists, widgetBoxIconLists, widgetBoxColumnLists, widgetBoxDescriptionLists, projectData, componentsData, mssqlTable]);

    return (
        <React.Fragment>
            <div style = {{ height:"100vh" }}>
                <div id="gjs"></div>
            </div>
        </React.Fragment >
    );
};

export default DashboardBuilder;