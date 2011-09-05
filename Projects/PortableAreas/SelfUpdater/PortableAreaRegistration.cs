﻿using System.Web.Mvc;
using MvcContrib.PortableAreas;
using Appleseed.Core;
using System.Reflection;
using Appleseed.Framework.Core.Model;

namespace SelfUpdater
{
    public class SelfUpdaterRegistration : MvcContrib.PortableAreas.PortableAreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "SelfUpdater";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context, IApplicationBus bus)
        {
            context.MapRoute("SelfUpdater_ResourceRoute", "SelfUpdater/resource/{resourceName}",
               new { controller = "EmbeddedResource", action = "Index" },
               new string[] { "MvcContrib.PortableAreas" });

            context.MapRoute("SelfUpdater_ResourceImageRoute", "SelfUpdater/images/{resourceName}",
              new { controller = "EmbeddedResource", action = "Index", resourcePath = "images" },
              new string[] { "MvcContrib.PortableAreas" });

            context.MapRoute("SelfUpdater.Core_ResourceScriptRoute", "SelfUpdater/scripts/{resourceName}",
               new { controller = "EmbeddedResource", action = "Index", resourcePath = "Scripts" },
               new string[] { "MvcContrib.PortableAreas" });

            context.MapRoute(
                "SelfUpdater_default",
                "SelfUpdater/{controller}/{action}/{id}",
                new { action = "Index", controller = "Updates", id = UrlParameter.Optional }
            );

            var assemblyName = Assembly.GetAssembly(this.GetType()).FullName;


            var generalModuleDefId = ModelServices.RegisterPortableAreaModule(AreaName, assemblyName, "Installation");
            var moduleDefId = ModelServices.AddModuleToPortal(generalModuleDefId, 0);
            ModelServices.AddModuleToPage(moduleDefId, 180, "Available Packages", false);

            generalModuleDefId = ModelServices.RegisterPortableAreaModule(AreaName, assemblyName, "Updates");
            moduleDefId = ModelServices.AddModuleToPortal(generalModuleDefId, 0);
            ModelServices.AddModuleToPage(moduleDefId, 180, "Packages Updates", false);


            RegisterAreaEmbeddedResources();
            PortableAreaUtils.RegisterScripts(this, context, bus);
        }

    }
}
