// src/main.js
import React from 'react'
import { createRoot } from 'react-dom/client'

import Counter from './components/Counter.jsx'

// Global mount function for Razor to call
window.mountReactCounter = (containerId, initialValue = 0) => {
  const container = document.getElementById(containerId)
  if (!container) return

  const root = createRoot(container)
  root.render(<Counter initialValue={initialValue} />)
}