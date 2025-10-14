import { configureStore } from '@reduxjs/toolkit';
import leaguesReducer from '@/features/footballSlice'

export const store = configureStore({
  reducer: {
    leagues: leaguesReducer
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;