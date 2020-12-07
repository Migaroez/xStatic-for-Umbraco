﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
using XStatic.Deploy;
using XStatic.Generator;
using XStatic.Generator.Storage;
using XStatic.Generator.Transformers;
using XStatic.Plugin.Repositories;

namespace XStatic.Plugin.Controllers
{
    [PluginController("xstatic")]
    public class DeployController : UmbracoAuthorizedJsonController
    {
        private readonly IStaticSiteStorer _storer;
        private SitesRepository _sitesRepo;

        public DeployController(IStaticSiteStorer storer)
        {
            _storer = storer;
            _sitesRepo = new SitesRepository();
        }

        [HttpGet]
        public async Task<string> DeployStaticSite(int staticSiteId)
        {
            var entity = _sitesRepo.Get(staticSiteId);

            if(entity == null)
            {
                throw new HttpException(404, "Site not found with id " + staticSiteId);
            }

            var path = _storer.GetStorageLocationOfSite(entity.Id);

            if (!Directory.Exists(path))
            {
                throw new FileNotFoundException();
            }

            var deployer = new NetlifyDeployer();

            var results = await deployer.DeployWholeSite(path);

            // TODO: Update Last Deploy

            return results.WasSuccessful.ToString();
        }
    }
}