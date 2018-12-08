using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AppNetCoreWebAPIDAE2.Data;
using Newtonsoft.Json;
using AppNetCoreWebAPIDAE2.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AppNetCoreWebAPIDAE2.Controllers
{
    [Route("api/")]
    [ApiController]
    public class DAEController : Controller
    {
        private readonly DBContext LoDBContext;

        public DAEController(DBContext PaDBContext) {
            LoDBContext = PaDBContext;
        }

        public IActionResult Index() {
            return null;
        }

        [HttpGet]
        [Route("GetEdificios")]
        public ContentResult GetEdificios()
        {
            var res = from CE in LoDBContext.eva_cat_edificios
                      select new
                      {
                          CE.IdEdificio,
                          CE.DesEdificio,
                          CE.Alias,
                          CE.Prioridad,
                          CE.Clave,
                          CE.UsuarioReg,
                          CE.UsuarioMod,
                          CE.FechaReg,
                          CE.FechaUltMod,
                          CE.Activo,
                          CE.Borrado

                      };
            string result = JsonConvert.SerializeObject(res);
            return Content(result, "application/json");
        }

        // POST
        [HttpPost]
        [Route("AddEdificios")]
        public IActionResult Create(eva_cat_edificios ed)
        {       
            LoDBContext.eva_cat_edificios.Add(ed);
            LoDBContext.SaveChanges();
            return new ObjectResult("Correcto!");                         
        }

        // POST2
        [HttpPost]
        [Route("ExpEdificios")]
        public IActionResult ExpEdificios([FromBody] IEnumerable<eva_cat_edificios> ed)
        {
            
            foreach (eva_cat_edificios edi in ed)
            {
                var edificio = LoDBContext.eva_cat_edificios.Find(edi.IdEdificio);
                if (edificio != null) {
                    LoDBContext.Entry(edi).State = EntityState.Modified;
                    LoDBContext.SaveChanges();
                    LoDBContext.Entry(edi).State = EntityState.Detached;
                }
                else
                {
                    if(ModelState.IsValid)
                    {
                        LoDBContext.eva_cat_edificios.Add(edi);
                        LoDBContext.SaveChanges();
                        LoDBContext.Entry(edi).State = EntityState.Detached;
                    }
                }
            }
            //LoDBContext.eva_cat_edificios.Add(ed);
            //LoDBContext.SaveChanges();
            return new ObjectResult("Correcto!");
        }


        //UPDATE
        // PUT api/5
        [HttpPut("{id}")]        
        public IActionResult Update(Int16 id, eva_cat_edificios ed) {
            var edificio = LoDBContext.eva_cat_edificios.Find(id);

            if (edificio == null) { return new ObjectResult("Incorrecto!"); }

            edificio.Alias = ed.Alias;
            edificio.DesEdificio = ed.DesEdificio;
            edificio.Prioridad = ed.Prioridad;
            edificio.Clave = ed.Clave;
            edificio.FechaReg = ed.FechaReg;
            edificio.FechaUltMod = ed.FechaUltMod;
            edificio.UsuarioReg = ed.UsuarioReg;
            edificio.UsuarioMod = ed.UsuarioMod;
            edificio.Activo = ed.Activo;
            edificio.Borrado = ed.Borrado;

            LoDBContext.eva_cat_edificios.Update(edificio);
            LoDBContext.SaveChanges();
            return new ObjectResult("Correcto!");
        }

        // DELETE api/5
        [HttpDelete("{id}")]
        public IActionResult Delete(Int16 id)
        {
            var edificio = LoDBContext.eva_cat_edificios.Find(id);
            if (edificio == null) { return new ObjectResult("Incorrecto!"); }
            LoDBContext.eva_cat_edificios.Remove(edificio);
            LoDBContext.SaveChanges();
            return new ObjectResult("Correcto!");
        }
        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(Int16 id)
        {
            var edificio = LoDBContext.eva_cat_edificios.Find(id);
            if (edificio == null) { return new ObjectResult("No encontrado"); }
            string result = JsonConvert.SerializeObject(edificio);
            return Content(result, "application/json");
        }

        [HttpGet]
        [Route("GetEdificiosWithEspacios")]
        public ContentResult GetEdifiEspa()
        {
            var resultado = from ece in LoDBContext.eva_cat_edificios
                            join eces in LoDBContext.eva_cat_espacios 
                            on ece.IdEdificio equals eces.IdEdificio
                            select new
                            {
                                ece.IdEdificio,
                                ece.DesEdificio,
                                ece.Activo,
                                ece.FechaReg,
                                eces.Clave,
                                eces.Alias,
                                eces.DesEspacio,                                
                            };
            var resultadoGroup = resultado.GroupBy(re => re.IdEdificio)
                .Select(group => new {
                    IdEdificio = group.First().IdEdificio,
                    DesEdificio = group.First().DesEdificio,
                    Activo = group.First().Activo,
                    FechaReg = group.First().FechaReg,
                    espacios = group.Select(ec => new {
                        Clave = ec.Clave,
                        IdClave = ec.Alias,
                        ec.Activo,
                        ec.DesEspacio
                    })
                });
            string json = JsonConvert.SerializeObject(resultadoGroup);
            return Content(json, "application/json");
        }
        [HttpGet]
        [Route("GetComCon")]
        public ContentResult GetComCon(){
            var res = from tc in LoDBContext.eva_cat_tipo_competencias
                      join cc in LoDBContext.eva_cat_competencias on tc.IdTipoCompetencia equals cc.IdTipoCompetencia
                      join co in LoDBContext.eva_cat_conocimientos on cc.IdCompetencia equals co.IdCompetencia
                      select new {
                          tc.IdTipoCompetencia, tc.DesTipoCompetecia,
                          cc.IdCompetencia, cc.DesCompetencia,
                          co.IdConocimiento, co.DesConocimiento
                      };
            var resGroup = res.GroupBy(re => re.IdTipoCompetencia)
                .Select(group => new {
                    IdTipoCompetencia = group.First().IdTipoCompetencia,
                    DesTipoCompetencia = group.First().DesTipoCompetecia,
                    competencias = group.Select(com => new {
                        com.IdCompetencia,
                        com.DesCompetencia
                    }),
                    conocimiento = group.Select(con => new {
                        con.IdConocimiento,
                        con.DesConocimiento
                    })
                });


            string json = JsonConvert.SerializeObject(resGroup);
            return Content(json, "application/json");
        }


    }
}
