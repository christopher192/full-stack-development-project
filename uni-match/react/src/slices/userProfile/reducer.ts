import { createSlice } from "@reduxjs/toolkit";
import { getTenUserProfileData } from './thunk';

export const initialState: any = {
    userProfileList: [],
    error: {},
};

const UserProfileSlice: any = createSlice({
    name: 'UserProfileSlice',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder.addCase(getTenUserProfileData.fulfilled, (state: any, action: any) => {
            state.userProfileList = action.payload;
        });
        builder.addCase(getTenUserProfileData.rejected, (state: any, action: any) => {
            state.error = action.payload.error || null;
        });
    }
});

export default UserProfileSlice.reducer;