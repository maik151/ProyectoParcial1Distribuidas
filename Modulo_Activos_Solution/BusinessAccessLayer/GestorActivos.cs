using System;
using System.Linq;
using Entity;
using DataAccesLayer;
using Microsoft.Extensions.Configuration;

namespace BusinessAccessLayer
{
    public class GestorActivos
    {
        private readonly TipoActivoDao _tipoDao;
        private readonly ActivoDao _activoDao;
        private readonly DepreciacionDao _depDao;

        public GestorActivos(IConfiguration config)
        {
            var db = new AccesoDatos(config);
            _tipoDao = new TipoActivoDao(db);
            _activoDao = new ActivoDao(db);
            _depDao = new DepreciacionDao(db);
        }

        // -------- TIPOS ----------
        public ListaTipoActivoResponse ListarTipos(TipoActivoRequest req)
        {
            var resp = new ListaTipoActivoResponse();
            try
            {
                resp.Tipos = _tipoDao.Listar(req.FiltroNombre ?? "");
                resp.Exito = true;
                resp.Mensaje = "OK";
            }
            catch (Exception ex)
            {
                resp.Exito = false;
                resp.Mensaje = ex.Message;
            }
            return resp;
        }

        public RespuestaBase GuardarTipo(TipoActivoRequest req)
        {
            var resp = new RespuestaBase();
            try
            {
                if (req.TipoActivo == null) throw new Exception("TipoActivo vacío.");

                // Si existe -> actualizar; si no existe -> insertar
                var existentes = _tipoDao.Listar(req.TipoActivo.Nombre ?? "");
                bool existe = existentes.Any(x => x.Codigo == req.TipoActivo.Codigo);

                if (!existe)
                    _tipoDao.Insertar(req.TipoActivo);
                else
                    _tipoDao.Actualizar(req.TipoActivo);

                resp.Exito = true;
                resp.Mensaje = "Guardado correctamente";
            }
            catch (Exception ex)
            {
                resp.Exito = false;
                resp.Mensaje = ex.Message;
            }
            return resp;
        }

        public RespuestaBase EliminarTipo(TipoActivoRequest req)
        {
            var resp = new RespuestaBase();
            try
            {
                if (req.TipoActivo == null) throw new Exception("TipoActivo vacío.");
                _tipoDao.Eliminar(req.TipoActivo.Codigo);
                resp.Exito = true;
                resp.Mensaje = "Eliminado";
            }
            catch (Exception ex)
            {
                resp.Exito = false;
                resp.Mensaje = ex.Message;
            }
            return resp;
        }

        // -------- ACTIVOS ----------
        public ListaActivosResponse ListarActivos(ActivoRequest req)
        {
            var resp = new ListaActivosResponse();
            try
            {
                resp.Activos = _activoDao.Listar(req.FiltroNombre ?? "");
                resp.Exito = true;
                resp.Mensaje = "OK";
            }
            catch (Exception ex)
            {
                resp.Exito = false;
                resp.Mensaje = ex.Message;
            }
            return resp;
        }

        public RespuestaBase GuardarActivo(ActivoRequest req)
        {
            var resp = new RespuestaBase();
            try
            {
                if (req.Activo == null) throw new Exception("Activo vacío.");

                if (req.Activo.ActivoId == 0)
                    _activoDao.Insertar(req.Activo);
                else
                    _activoDao.Actualizar(req.Activo);

                resp.Exito = true;
                resp.Mensaje = "Guardado correctamente";
            }
            catch (Exception ex)
            {
                resp.Exito = false;
                resp.Mensaje = ex.Message;
            }
            return resp;
        }

        public RespuestaBase EliminarActivo(ActivoRequest req)
        {
            var resp = new RespuestaBase();
            try
            {
                if (req.Activo == null) throw new Exception("Activo vacío.");
                _activoDao.Eliminar(req.Activo.ActivoId);
                resp.Exito = true;
                resp.Mensaje = "Eliminado";
            }
            catch (Exception ex)
            {
                resp.Exito = false;
                resp.Mensaje = ex.Message;
            }
            return resp;
        }

        // -------- DEPRECIACIÓN ----------
        public DepreciacionResponse DepreciarMes(DepreciacionRequest req)
        {
            var resp = new DepreciacionResponse();

            try
            {
                var cab = new DepreciacionCabeceraDTO
                {
                    FechaProceso = req.FechaProceso,
                    Observaciones = req.Observaciones,
                    UsuarioEjecucion = req.UsuarioEjecucion
                };

                long depId = _depDao.InsertarCabecera(cab);

                var activos = _activoDao.Listar("");

                short periodo = 1;
                foreach (var a in activos)
                {
                    decimal valorMensual = a.ValorCompra / a.PeriodosDepreciacionTotal;

                    var det = new DepreciacionDetalleDTO
                    {
                        DepreciacionId = depId,
                        ActivoId = a.ActivoId,
                        PeriodoNumero = periodo,
                        ValorDepreciado = valorMensual
                    };

                    _depDao.InsertarDetalle(det);
                }

                resp.Exito = true;
                resp.Mensaje = "Depreciación generada";
                resp.DepreciacionId = depId;
            }
            catch (Exception ex)
            {
                resp.Exito = false;
                resp.Mensaje = ex.Message;
            }

            return resp;
        }
    }
}

