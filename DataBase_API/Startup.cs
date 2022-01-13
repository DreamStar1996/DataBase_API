using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace DataBase_API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //配置跨域处理，允许所有来源
            services.AddCors(options =>
            {
                options.AddPolicy("cors",
                    builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
                );
            });

            #region 添加Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V1", new OpenApiInfo
                {
                    Version = "V1",   //版本 
                    Title = $"XUnit.Core 接口文档-NetCore3.1",  //标题
                    Description = $"XUnit.Core Http API V1",    //描述
                    Contact = new OpenApiContact { Name = "DreamStaro", Email = "", Url = new Uri("https://seachen.cn") },
                    License = new OpenApiLicense { Name = "DreamStaro许可证", Url = new Uri("https://seachen.cn") }
                });
                // 获取xml文件名
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // 获取xml文件路径
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                // 添加控制器层注释，true表示显示控制器注释
                c.IncludeXmlComments(xmlPath, true);
            });
            #endregion

            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddControllersWithViews();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // 添加Swagger有关中间件
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((doc, item) =>
                {
                    //根据代理服务器提供的协议、地址和路由，生成api文档服务地址
                    doc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{item.Scheme}://{item.Host.Value}/{item.Headers["X-Forwarded-Prefix"]}" } };
                });
            });
            app.UseSwaggerUI(c =>
            {
                c.IndexStream = () => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("DataBase_API.index.html");
                c.RoutePrefix = string.Empty;     //如果是为空 访问路径就为 根域名/index.html,注意localhost:8001/swagger是访问不到的
                c.SwaggerEndpoint($"/swagger/V1/swagger.json", $"XUnit.Core V1");
                //路径配置，设置为空，表示直接在根域名（localhost:8001）访问该文件
                c.RoutePrefix = "swagger"; // 如果你想换一个路径，直接写名字即可，比如直接写c.RoutePrefix = "swagger"; 则访问路径为 根域名/swagger/index.html
            });

            app.UseRouting();

            //允许所有跨域，cors是在ConfigureServices方法中配置的跨域策略名称
            //注意：UseCors必须放在UseRouting和UseEndpoints之间
            app.UseCors("cors");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //跨域需添加RequireCors方法，cors是在ConfigureServices方法中配置的跨域策略名称
                endpoints.MapControllers().RequireCors("cors");
            });
        }
    }
}
