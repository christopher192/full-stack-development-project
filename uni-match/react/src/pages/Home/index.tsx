import React, { useState, useEffect, useRef } from 'react';
import { Link } from "react-router-dom";
import { 
  Card,
  CardBody,
  Col,
  Row,
  Form,
  Input,
  UncontrolledDropdown,
  DropdownItem,
  DropdownMenu,
  DropdownToggle,
} from "reactstrap";
import Flatpickr from "react-flatpickr";
import BreadCrumb from "../../Components/Common/BreadCrumb";
import { jobGrid } from "../../common/data/appsJobs";
import { useSpring, animated, to } from '@react-spring/web'
import { useGesture, useDrag } from '@use-gesture/react'
import useMeasure from 'react-use-measure'
import { useSelector, useDispatch } from "react-redux";
import { getTenUserProfileData } from '../../slices/thunks';
import { createSelector } from 'reselect';

const Home = () => {
  document.title = "Home";

  const dispatch: any = useDispatch();
  const selectUserProfileData = createSelector(
    (state: any) => state.UserProfile,
    (userProfileList) => userProfileList.userProfileList
  );

  const [userProfileData, setUserProfileData] = useState<any>([]);

  const [showElement, setShowElement] = useState(true);
  const [{ border, width, height, touchAction, x, y }, api] = useSpring(() => ({ border: '1px solid black', width: '250px', height: '250px', touchAction: 'none',  x: 0, y: 0 }));
  const bind = useGesture({
    onDrag: ({ down, movement: [mx, my], velocity }) => { 
      api.start({ x: down ? mx : 0, y: down ? my : 0, immediate: down });

      // ignore swipe up and down
      if (Math.abs(my) > Math.abs(mx)) {
        return;
      }

      const isSwipeLeft = velocity[0] > 0.5 && mx < 0;
      const isSwipeRight = velocity[0] > 0.5 && mx > 0;

      if (isSwipeLeft) {
        console.log('Swipe left detected');
      } else if (isSwipeRight) {
        console.log('Swipe right detected');
      }

      // drag has stopped
      if (!down) {
        console.log('Drag has stopped');
        // hide the element
        setShowElement(false);
      }
    },
  });

  const getUserProfileData = useSelector(selectUserProfileData);

  useEffect(() => {
    setUserProfileData(getUserProfileData);
  }, [getUserProfileData]); 

  useEffect(() => {
    dispatch(getTenUserProfileData());
  }, [dispatch]);

  return (
    <React.Fragment>
      <div className = "page-content">
        <div className = "container-fluid">
          <BreadCrumb title = "Recommand List" pageTitle = "Dashboard" />
          <Row>
            <Col lg = {6}>
              {userProfileData.map((data: any, index: number) => 
                showElement && (
                  <animated.div key = {index} style = {{ border, width, height, touchAction, x, y }} {...bind()}>
                    <h1>{data.id}</h1>
                  </animated.div>
                )
              )}
            </Col>
          </Row>
        </div>
      </div>
    </React.Fragment>
  );
};

export default Home;