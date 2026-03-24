import { BrowserRouter, Routes, Route } from 'react-router-dom'
import { TodoPage } from './pages/todos/todo-page'

export function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<TodoPage />} />
      </Routes>
    </BrowserRouter>
  )
}
