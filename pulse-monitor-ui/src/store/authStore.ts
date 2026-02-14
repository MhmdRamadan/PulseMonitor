import { create } from 'zustand'
import { persist } from 'zustand/middleware'

interface AuthState {
  accessToken: string | null
  userName: string | null
  roles: string[]
  setAuth: (token: string, userName: string, roles: string[]) => void
  logout: () => void
  isAdmin: () => boolean
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      accessToken: null,
      userName: null,
      roles: [],
      setAuth: (accessToken, userName, roles) => {
        localStorage.setItem('accessToken', accessToken)
        set({ accessToken, userName, roles })
      },
      logout: () => {
        localStorage.removeItem('accessToken')
        set({ accessToken: null, userName: null, roles: [] })
      },
      isAdmin: () => get().roles.includes('Admin'),
    }),
    { name: 'auth', partialize: (s) => ({ accessToken: s.accessToken, userName: s.userName, roles: s.roles }) }
  )
)
