import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
/*import './index.css' dont uncomment this is default vite css*/
import App from './App.jsx'
//this just builds our app
createRoot(document.getElementById('root')).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
