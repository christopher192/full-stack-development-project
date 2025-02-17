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
import { useSpring, useSprings, animated, to } from '@react-spring/web'
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
  const [showElements, setShowElements] = useState(Array(userProfileData.length).fill(true));
  const isSwipeRightRef = useRef(false);

  // const [spring, api] = useSpring(() => ({ border: '1px solid black', width: '250px', height: '250px', touchAction: 'none',  x: 0, y: 0 }));
  const [springs, apis] = useSprings(userProfileData.length, index => ({ border: '1px solid black', width: '250px', height: '250px', touchAction: 'none',  x: 0, y: 0 }));
  const bind = useGesture({
    onDrag: ({ args: [originalIndex], down, movement: [mx, my], velocity }) => {
      // api.start({ x: down ? mx : 0, y: down ? my : 0, immediate: down });
      apis.start(i => {
        if (i === originalIndex){
          return { x: down ? mx : 0, y: down ? my : 0, immediate: down };
        }
      });
      // ignore swipe up and down
      if (Math.abs(my) > Math.abs(mx)) {
        return;
      }

      const isSwipeLeft = velocity[0] > 0.5 && mx < 0;
      const isSwipeRight = velocity[0] > 0.5 && mx > 0;

      if (isSwipeLeft) {
        isSwipeRightRef.current = false;
      } else if (isSwipeRight) {
        isSwipeRightRef.current = isSwipeRight;
      }

      // drag has stopped
      if (!down) {
        // hide the element
        // setShowElement(false);
        if (isSwipeRightRef.current) {
          setTimeout(() => {
            const copyShowElements = [...showElements];
            copyShowElements[originalIndex] = false;
            setShowElements(copyShowElements);
          }, 1200);
        }
        // reset the ref for the next swipe
        isSwipeRightRef.current = false;
      }
    }
  });

  const getUserProfileData = useSelector(selectUserProfileData);

  useEffect(() => {
    setUserProfileData(getUserProfileData);
    setShowElements(Array(getUserProfileData.length).fill(true));
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
              {/* {userProfileData.map((data: any, index: number) => 
                showElement && (
                  <animated.div key = {index} style = { spring } {...bind(index)}>
                    <h1>{data.id}</h1>
                  </animated.div>
                )
              )} */}
              {userProfileData.length > 0 && springs.map((spring, index) => (
                showElements[index] && (<Col lg = {4} key = {index}>
                  <animated.div style = { spring } {...bind(index)}>
                    <h1>{index}</h1>
                  </animated.div>
                </Col>)
              ))}
          </Row>
        </div>
      </div>
    </React.Fragment>
  );
};

export default Home;