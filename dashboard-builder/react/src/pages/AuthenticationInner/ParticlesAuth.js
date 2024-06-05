import React from 'react';
import withRouter from '../../Components/Common/withRouter';

const ParticlesAuth = ({ children }) => {
    return (
        <React.Fragment>
            <div className="auth-page-wrapper auth-bg-cover py-5 d-flex justify-content-center align-items-center min-vh-100">
                <div className="bg-overlay"></div>
                {children}
            </div>
        </React.Fragment>
    );
};

export default withRouter(ParticlesAuth);