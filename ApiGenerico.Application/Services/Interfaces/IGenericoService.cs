using ApiGenerico.Domain.Models.Farma.SP;
using ApiGenerico.Domain.Models.Dto;
using ApiGenerico.Domain.Models.Farma.Core;
using Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGenerico.Application.Services.Interfaces
{
    public interface IGenericoService
    {
        #region POST
        ResultOperation<bool> PostUsuarioAdministrativo(UsuarioAdministrativoPost Usuario);
        #endregion

        #region GET
        ResultOperation<GenericoResponse> GetUsuarioAdministrativo(QueryFilter entity);
        #endregion

        #region UPDATE
        ResultOperation<bool> UpdateUsuarioAdministrativo(UsuarioAdministrativoUpdate Usuario);
        #endregion

        #region DELETE
        ResultOperation<bool> DeleteUsuarioAdministrativo(int idUsuario);
        #endregion
    }
}
