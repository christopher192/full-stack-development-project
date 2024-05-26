import { createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";

export const getTenUserProfileData = createAsyncThunk("userProfile/getTenUserProfileData", async (data: any) => {
    try {
        const response = await axios.get("http://localhost:3000/users");
        return response;
    } catch (error) {
        return error;
    }
});