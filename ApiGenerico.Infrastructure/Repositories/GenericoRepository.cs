

using ApiGenerico.Domain.Models.Farma.SP;
using ApiGenerico.Domain.Models.Farma.SP;
using ApiGenerico.Infrastructure.Repositories._UnitOfWork;
using ApiGenerico.Infrastructure.Repositories.Interfaces;
using Dapper;

namespace ApiGenerico.Infrastructure.Repositories
{
    public class GenericoRepository : Repository, IGenericoRepository
    {
        public GenericoRepository() { }
        public GenericoRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        #region POST
        public bool PostUsuarioAdministrativo(UsuarioAdministrativoPost usuario)
        {
            var prms = new DynamicParameters();
            prms.Add("IdSede", usuario.IdSede);
            prms.Add("strNombreUsuario", usuario.NombreUsuario);
            prms.Add("strContrasena", usuario.Contrasena);
            prms.Add("strUsuarioRed", usuario.UsuarioRed);
            prms.Add("CorreoElectronico", usuario.CorreoElectronico);
            prms.Add("Activo", usuario.Activo);
            var response = Execute<int>("SpVt_QueryPostUsuarioAdministrador", prms) > 0 ? true : false;
            return response;
        }
        #endregion

        #region GET
        public List<UsuarioAdministrativo> GetUsuarioAdministrativo()
        {
            var response = GetDataListOfProcedure<UsuarioAdministrativo>("SpVt_QueryGetUsuarioAdministrativo");
            return response;
        }
        #endregion

        #region UPDATE
        public bool UpdateUsuarioAdministrativo(UsuarioAdministrativoUpdate usuario)
        {
            var prms = new DynamicParameters();
            prms.Add("IdUsuario", usuario.IdUsuario);

            if (usuario.IdSede.HasValue)
            {
                prms.Add("IdSede", usuario.IdSede);
            }
            if (!string.IsNullOrEmpty(usuario.NombreUsuario))
            {
                prms.Add("strNombreUsuario", usuario.NombreUsuario);
            }
            if (!string.IsNullOrEmpty(usuario.Contrasena))
            {
                prms.Add("strContrasena", usuario.Contrasena);
            }
            if (!string.IsNullOrEmpty(usuario.UsuarioRed))
            {
                prms.Add("strUsuarioRed", usuario.UsuarioRed);
            }
            if (!string.IsNullOrEmpty(usuario.CorreoElectronico))
            {
                prms.Add("CorreoElectronico", usuario.CorreoElectronico);
            }
            if (usuario.Activo.HasValue)
            {
                prms.Add("Activo", usuario.Activo);
            }

            var response = Execute<int>("SpVt_QueryUpdateUsuarioAdministrador", prms);
            return response > 0;
        }
        #endregion

        #region DELETE
        public bool DeleteUsuarioAdministrativo(int idUsuario)
        {
            var prms = new DynamicParameters();
            prms.Add("idUsuario", idUsuario);
            var response = Execute<int>("SpVt_QueryDeleteUsuarioAdministrativo", prms) > 0 ? true : false;
            return response;
        }
        #endregion
    }
}
