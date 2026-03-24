import { create } from 'zustand'

interface UserUiState {
  selectedUserId: number | null
  selectUser: (id: number) => void
}

export const useUserStore = create<UserUiState>((set) => ({
  selectedUserId: null,
  selectUser: (id) => set({ selectedUserId: id }),
}))
