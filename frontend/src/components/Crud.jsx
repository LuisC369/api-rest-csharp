import { useCallback, useEffect, useState } from 'react'
import { api } from '../api'

/**
 * Tabla + formulario reutilizable para hacer SELECT, INSERT, UPDATE y DELETE de una tabla.
 *
 * - recurso:  nombre del controlador en la API (Clientes, Empleados, Puestos)
 * - singular: nombre en singular usado en las rutas (Cliente → CrearCliente, ActualizarCliente...)
 * - idCampo:  propiedad que trae el id en el JSON (idCliente, idEmpleado...)
 * - columnas: columnas a mostrar en la tabla
 * - campos:   campos del formulario (type "text" o "select")
 */
export default function Crud({ recurso, singular, idCampo, columnas, campos, onCambio }) {
  const formularioVacio = Object.fromEntries(campos.map((c) => [c.name, '']))

  const [filas, setFilas] = useState([])
  const [formulario, setFormulario] = useState(formularioVacio)
  const [editandoId, setEditandoId] = useState(null)
  const [cargando, setCargando] = useState(true)
  const [mensaje, setMensaje] = useState(null)

  const cargar = useCallback(async () => {
    setCargando(true)
    try {
      setFilas(await api(`/${recurso}/Listar${recurso}`))
    } catch (error) {
      setMensaje({ tipo: 'error', texto: error.message })
    } finally {
      setCargando(false)
    }
  }, [recurso])

  useEffect(() => {
    cargar()
  }, [cargar])

  function avisar(tipo, texto) {
    setMensaje({ tipo, texto })
    if (tipo === 'ok') setTimeout(() => setMensaje(null), 3000)
  }

  function cancelar() {
    setEditandoId(null)
    setFormulario(formularioVacio)
  }

  function editar(fila) {
    setEditandoId(fila[idCampo])
    setFormulario(Object.fromEntries(campos.map((c) => [c.name, String(fila[c.name] ?? '')])))
    setMensaje(null)
  }

  async function guardar(evento) {
    evento.preventDefault()

    // Convierte los campos numéricos (como idPuesto) antes de enviarlos
    const cuerpo = Object.fromEntries(
      campos.map((c) => [c.name, c.numerico ? Number(formulario[c.name]) : formulario[c.name].trim()]),
    )

    try {
      if (editandoId) {
        await api(`/${recurso}/Actualizar${singular}/${editandoId}`, 'PUT', cuerpo)
        avisar('ok', `${singular} #${editandoId} actualizado`)
      } else {
        const creado = await api(`/${recurso}/Crear${singular}`, 'POST', cuerpo)
        avisar('ok', `${singular} #${creado[idCampo]} creado`)
      }
      cancelar()
      await cargar()
      onCambio?.()
    } catch (error) {
      avisar('error', error.message)
    }
  }

  async function eliminar(fila) {
    const id = fila[idCampo]
    if (!window.confirm(`¿Eliminar ${singular.toLowerCase()} #${id}?`)) return

    try {
      await api(`/${recurso}/Eliminar${singular}/${id}`, 'DELETE')
      avisar('ok', `${singular} #${id} eliminado`)
      if (editandoId === id) cancelar()
      await cargar()
      onCambio?.()
    } catch (error) {
      avisar('error', error.message)
    }
  }

  return (
    <section className="crud">
      <form className="tarjeta formulario" onSubmit={guardar}>
        <h2>{editandoId ? `Editar ${singular.toLowerCase()} #${editandoId}` : `Nuevo ${singular.toLowerCase()}`}</h2>

        {campos.map((campo) => (
          <label key={campo.name}>
            {campo.label}
            {campo.type === 'select' ? (
              <select
                required
                value={formulario[campo.name]}
                onChange={(e) => setFormulario({ ...formulario, [campo.name]: e.target.value })}
              >
                <option value="">Selecciona...</option>
                {campo.options.map((op) => (
                  <option key={op.value} value={op.value}>
                    {op.label}
                  </option>
                ))}
              </select>
            ) : (
              <input
                required
                maxLength={50}
                value={formulario[campo.name]}
                onChange={(e) => setFormulario({ ...formulario, [campo.name]: e.target.value })}
              />
            )}
            {campo.ayuda && <small>{campo.ayuda}</small>}
          </label>
        ))}

        <div className="acciones">
          <button type="submit" className="primario">
            {editandoId ? 'Guardar cambios' : 'Crear'}
          </button>
          {editandoId && (
            <button type="button" onClick={cancelar}>
              Cancelar
            </button>
          )}
        </div>
      </form>

      <div className="tarjeta tabla">
        <div className="tabla-encabezado">
          <h2>{recurso}</h2>
          <button type="button" onClick={cargar}>
            ↻ Recargar
          </button>
        </div>

        {mensaje && <p className={`mensaje ${mensaje.tipo}`}>{mensaje.texto}</p>}

        {cargando ? (
          <p className="vacio">Cargando...</p>
        ) : filas.length === 0 ? (
          <p className="vacio">No hay registros todavía.</p>
        ) : (
          <div className="scroll">
            <table>
              <thead>
                <tr>
                  {columnas.map((col) => (
                    <th key={col.key}>{col.label}</th>
                  ))}
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {filas.map((fila) => (
                  <tr key={fila[idCampo]} className={editandoId === fila[idCampo] ? 'activa' : ''}>
                    {columnas.map((col) => (
                      <td key={col.key}>{fila[col.key]}</td>
                    ))}
                    <td className="botones-fila">
                      <button type="button" onClick={() => editar(fila)}>
                        Editar
                      </button>
                      <button type="button" className="peligro" onClick={() => eliminar(fila)}>
                        Eliminar
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </section>
  )
}
