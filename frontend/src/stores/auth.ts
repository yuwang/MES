import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface UserInfo {
  username: string;
  realName: string;
  roleName: string;
}

interface AuthStore {
  token: string | null;
  userInfo: UserInfo | null;
  setAuth: (token: string, userInfo: UserInfo) => void;
  clearAuth: () => void;
  isAuthenticated: () => boolean;
}

export const useAuthStore = create<AuthStore>()(
  persist(
    (set, get) => ({
      token: null,
      userInfo: null,
      setAuth: (token, userInfo) => {
        localStorage.setItem('token', token);
        set({ token, userInfo });
      },
      clearAuth: () => {
        localStorage.removeItem('token');
        set({ token: null, userInfo: null });
      },
      isAuthenticated: () => {
        return !!get().token;
      },
    }),
    {
      name: 'auth-storage',
    }
  )
);
