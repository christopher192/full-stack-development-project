import React, { useState, useEffect } from 'react';
import { useSelector } from 'react-redux';
import { Dropdown, DropdownItem, DropdownMenu, DropdownToggle } from 'reactstrap';
import { Link } from 'react-router-dom';

//import images
import avatar1 from "../../assets/images/users/avatar-1.jpg";

const ProfileDropdown = () => {
    const { user } = useSelector(state => ({
        user: state.Profile.user,
    }));

    const [userName, setUserName] = useState(null);

    useEffect(() => {
        if (sessionStorage.getItem("authUser")) {
            const obj = JSON.parse(sessionStorage.getItem("authUser"));
            // setUserName(process.env.REACT_APP_DEFAULTAUTH === "fake" ? obj.username === undefined ? user.first_name ? user.first_name : obj.data.first_name : "Admin" || "Admin" :
            //     process.env.REACT_APP_DEFAULTAUTH === "firebase" ? obj.providerData[0].email : "Admin"
            // );
            setUserName("Admin");
        }
    }, [userName, user]);

    //Dropdown Toggle
    const [isProfileDropdown, setIsProfileDropdown] = useState(false);
    const toggleProfileDropdown = () => {
        setIsProfileDropdown(!isProfileDropdown);
    };
    return (
        <React.Fragment>
            {userName ? 
                (<Dropdown isOpen={isProfileDropdown} toggle={toggleProfileDropdown} className="ms-sm-3 header-item topbar-user">
                    <DropdownToggle tag="button" type="button" className="btn">
                        <span className="d-flex align-items-center">
                            <img className="rounded-circle header-profile-user" src={avatar1}
                                alt="Header Avatar" />
                            <span className="text-start ms-xl-2">
                                <span className="d-none d-xl-inline-block ms-1 fw-medium user-name-text">{userName}</span>
                                <span className="d-none d-xl-block ms-1 fs-12 text-muted user-name-sub-text">Founder</span>
                            </span>
                        </span>
                    </DropdownToggle>

                    <DropdownMenu className="dropdown-menu-end">
                        <h6 className="dropdown-header">Welcome {userName}!</h6>
                        <DropdownItem><i className="mdi mdi-account-circle text-muted fs-16 align-middle me-1"></i>
                            <span className="align-middle">Profile</span></DropdownItem>
                        <div className="dropdown-divider"></div>
                        <DropdownItem href={process.env.PUBLIC_URL + "/logout"}><i
                            className="mdi mdi-logout text-muted fs-16 align-middle me-1"></i> <span
                                className="align-middle" data-key="t-logout">Logout</span></DropdownItem>
                    </DropdownMenu>
                </Dropdown>)
                : (
                    <div className="topbar-user">
                        <Link to="/login" className="btn btn-primary" style={{backgroundColor: "#364574", borderColor: "#33416e"}}>Login</Link>
                    </div>
                )
            }


        </React.Fragment>
    );
};

export default ProfileDropdown;