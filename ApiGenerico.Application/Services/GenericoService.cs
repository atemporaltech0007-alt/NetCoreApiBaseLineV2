using ApiGenerico.Domain.Models.Farma.SP;
using ApiGenerico.Application.Services.Interfaces;
using ApiGenerico.Domain.Entities.CustomEntities;
using ApiGenerico.Domain.Models;
using ApiGenerico.Domain.Models.Dto;
using ApiGenerico.Domain.Models.Farma.Core;
using ApiGenerico.Domain.Models.Farma.SP;
using ApiGenerico.Infrastructure.Repositories;
using Microsoft.Extensions.Options;
using Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGenerico.Application.Services
{
    public class GenericoService : _Service, IGenericoService
    {
        public GenericoService(IOptions<ConnectionStrings> connectionStrings) : base(connectionStrings.Value.ConnetionGenerico)
        {

        }

        #region POST
        public ResultOperation<bool> PostUsuarioAdministrativo(UsuarioAdministrativoPost Usuario)
        {
            var result =
                WrapExecuteTrans<ResultOperation<bool>, GenericoRepository>((repo, uow) =>
                {
                    var rst = new ResultOperation<bool>();

                    try
                    {
                        rst.Result = repo.PostUsuarioAdministrativo(Usuario);
                        rst.stateOperation = true;
                    }
                    catch (Exception err)
                    {
                        rst.RollBack = true;
                        rst.stateOperation = false;
                        rst.MessageExceptionTechnical = err.Message + Environment.NewLine + err.StackTrace;
                    }

                    return rst;
                });

            return result;
        }
        #endregion

        #region GET
        public ResultOperation<GenericoResponse> GetUsuarioAdministrativo(QueryFilter entity)
        {
            var result =
                WrapExecuteTrans<ResultOperation<GenericoResponse>, GenericoRepository>((repo, uow) =>
                {
                    var rst = new ResultOperation<GenericoResponse>();
                    var UsuarioAdministrativoList = new List<UsuarioAdministrativo>();

                    try
                    {
                        UsuarioAdministrativoList = repo.GetUsuarioAdministrativo();
                        if (UsuarioAdministrativoList.Count == 0)
                        {
                            rst.MessageResult = "No hay datos";
                        }

                        var response = PagedList<UsuarioAdministrativo>.Create(UsuarioAdministrativoList, entity.PageNumber, 100000);
                        rst.Result = new GenericoResponse { PagedUsuarioAdministrativo = response };
                        rst.stateOperation = true;
                    }
                    catch (Exception err)
                    {
                        rst.RollBack = true;
                        rst.stateOperation = false;
                        rst.MessageExceptionTechnical = err.Message + Environment.NewLine + err.StackTrace;
                    }

                    return rst;
                });

            return result;
        }
        #endregion

        #region UPDATE
        public ResultOperation<bool> UpdateUsuarioAdministrativo(UsuarioAdministrativoUpdate Usuario)
        {
            var result = WrapExecuteTrans<ResultOperation<bool>, GenericoRepository>((repo, uow) =>
            {
                var rst = new ResultOperation<bool>();

                try
                {
                    rst.Result = repo.UpdateUsuarioAdministrativo(Usuario);
                    rst.stateOperation = true;
                }
                catch (Exception ex)
                {
                    rst.RollBack = true;
                    rst.stateOperation = false;
                    rst.MessageExceptionTechnical = $"{ex.Message}{Environment.NewLine}{ex.StackTrace}";
                }

                return rst;
            });

            return result;
        }
        #endregion

        #region DELETE
        public ResultOperation<bool> DeleteUsuarioAdministrativo(int idUsuario)
        {
            var result =
                WrapExecuteTrans<ResultOperation<bool>, GenericoRepository>((repo, uow) =>
                {
                    var rst = new ResultOperation<bool>();

                    try
                    {
                        rst.Result = repo.DeleteUsuarioAdministrativo(idUsuario);
                        rst.stateOperation = true;
                    }
                    catch (Exception err)
                    {
                        rst.RollBack = true;
                        rst.stateOperation = false;
                        rst.MessageExceptionTechnical = err.Message + Environment.NewLine + err.StackTrace;
                    }

                    return rst;
                });

            return result;
        }
        #endregion
    }
}
