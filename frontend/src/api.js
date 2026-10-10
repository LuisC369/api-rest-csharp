// URL base de la Web API en .NET (se puede cambiar con la variable VITE_API_URL)
const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5149'

// Llama a la API y devuelve el JSON de la respuesta, o lanza un Error con el mensaje del servidor
export async function api(ruta, metodo = 'GET', cuerpo) {
  let respuesta
  try {
    respuesta = await fetch(`${API_URL}${ruta}`, {
      method: metodo,
      headers: cuerpo ? { 'Content-Type': 'application/json' } : undefined,
      body: cuerpo ? JSON.stringify(cuerpo) : undefined,
    })
  } catch {
    throw new Error(`No se pudo conectar con la API en ${API_URL}. ¿Está corriendo el backend?`)
  }

  if (respuesta.status === 204) return null

  const texto = await respuesta.text()
  let datos = texto
  try {
    datos = texto ? JSON.parse(texto) : null
  } catch {
    // La API devolvió texto plano (por ejemplo, los mensajes de NotFound)
  }

  if (!respuesta.ok) {
    throw new Error(mensajeDeError(datos) || `Error ${respuesta.status}`)
  }

  return datos
}

// Extrae un mensaje legible de los distintos formatos de error de ASP.NET
function mensajeDeError(datos) {
  if (typeof datos === 'string') return datos
  if (datos?.errors) return Object.values(datos.errors).flat().join(' ')
  return datos?.title
}
