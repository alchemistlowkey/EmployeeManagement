// src/components/Counter.jsx
import React, { useState } from 'react'

export default function Counter({ initialValue }) {
  const [count, setCount] = useState(Number(initialValue) || 0)

  return (
    <div style={{ padding: '1rem', border: '1px solid #ccc', borderRadius: '8px' }}>
      <h2>Counter: {count}</h2>
      <button onClick={() => setCount(c => c + 1)}>
        +1
      </button>
      <button onClick={() => setCount(c => c - 1)} style={{ marginLeft: '1rem' }}>
        -1
      </button>
    </div>
  )
}