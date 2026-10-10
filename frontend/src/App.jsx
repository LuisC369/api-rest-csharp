import { useCallback, useEffect, useState } from 'react'
import { api } from './api'
import Crud from './components/Crud'

const PESTANAS = ['Clientes', 'Empleados', 'Puestos']

export default function App() {
  const [pestana, setPestana] = useState('Clientes')
  const [puestos, setPuestos] = useState([])

  // Los puestos se usan en el select del formulario de empleados
  const cargarPuestos = useCallback(async () => {
    try {
      setPuestos(await api('/Puestos/ListarPuestos'))
    } catch {
      setPuestos([])
    }
  }, [])

  useEffect(() => {
    cargarPuestos()
  }, [cargarPuestos])

  return (
    <div className="app">
      <header>
        <h1>DB Empresa</h1>
        <p>Frontend en React conectado a la Web API en .NET</p>
      </header>

      <nav className="pestanas">
        {PESTANAS.map((p) => (
          <button key={p} className={p === pestana ? 'activa' : ''} onClick={() => setPestana(p)}>
            {p}
          </button>
        ))}
      </nav>

      {pestana === 'Clientes' && (
        <Crud
          recurso="Clientes"
          singular="Cliente"
          idCampo="idCliente"
          columnas={[
            { key: 'idCliente', label: 'ID' },
            { key: 'nombre', label: 'Nombre' },
            { key: 'apellidos', label: 'Apellidos' },
            { key: 'telefono', label: 'Teléfono' },
          ]}
          campos={[
            { name: 'nombre', label: 'Nombre' },
            { name: 'apellidos', label: 'Apellidos' },
            { name: 'telefono', label: 'Teléfono' },
          ]}
        />
      )}

      {pestana === 'Empleados' && (
        <Crud
          recurso="Empleados"
          singular="Empleado"
          idCampo="idEmpleado"
          onCambio={cargarPuestos}
          columnas={[
            { key: 'idEmpleado', label: 'ID' },
            { key: 'nombre', label: 'Nombre' },
            { key: 'apellido', label: 'Apellido' },
            { key: 'puesto', label: 'Puesto' },
          ]}
          campos={[
            { name: 'nombre', label: 'Nombre' },
            { name: 'apellido', label: 'Apellido' },
            {
              name: 'idPuesto',
              label: 'Puesto',
              type: 'select',
              numerico: true,
              options: puestos.map((p) => ({ value: String(p.idPuesto), label: p.nombre })),
              ayuda: puestos.length === 0 ? 'Primero crea un puesto en la pestaña Puestos.' : null,
            },
          ]}
        />
      )}

      {pestana === 'Puestos' && (
        <Crud
          recurso="Puestos"
          singular="Puesto"
          idCampo="idPuesto"
          onCambio={cargarPuestos}
          columnas={[
            { key: 'idPuesto', label: 'ID' },
            { key: 'nombre', label: 'Nombre' },
          ]}
          campos={[{ name: 'nombre', label: 'Nombre' }]}
        />
      )}
    </div>
  )
}
