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
using StackExchange.Profiling.Storage;

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

            #region ���ÿ���������������Դ
            services.AddCors(options =>
            {
                options.AddPolicy("cors",
                    builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
                );
            });
            #endregion

            #region MiniProfiler����
            services.AddMiniProfiler(options =>
            {
                options.RouteBasePath = "/profiler";
                (options.Storage as MemoryCacheStorage).CacheDuration = TimeSpan.FromMinutes(60);
                options.SqlFormatter = new StackExchange.Profiling.SqlFormatters.InlineFormatter();
                options.TrackConnectionOpenClose = true;
                options.ColorScheme = StackExchange.Profiling.ColorScheme.Auto;
                options.EnableMvcFilterProfiling = true;
                options.EnableMvcViewProfiling = true;
            }).AddEntityFramework();
            #endregion

            #region ���Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V1", new OpenApiInfo
                {
                    Version = "V1",   //�汾 
                    Title = $"DataBase-NetCore3.1",  //����
                    Description = $"DataBase Https API V1",    //����
                    Contact = new OpenApiContact { Name = "DreamStaro", Email = "", Url = new Uri("https://seachen.cn") }
                });
                // ��ȡxml�ļ���
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // ��ȡxml�ļ�·��
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                // ��ӿ�������ע�ͣ�true��ʾ��ʾ������ע��
                c.IncludeXmlComments(xmlPath, true);
            });
            #endregion

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseMiniProfiler();

            // ���Swagger�й��м��
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((doc, item) =>
                {
                    //���ݴ���������ṩ��Э�顢��ַ��·�ɣ�����api�ĵ������ַ
                    doc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{item.Scheme}://{item.Host.Value}/{item.Headers["X-Forwarded-Prefix"]}" } };
                });
            });
            app.UseSwaggerUI(c =>
            {
                c.IndexStream = () => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("DataBase_API.index.html");
                c.RoutePrefix = string.Empty;     //�����Ϊ�� ����·����Ϊ ������/index.html,ע��localhost:8001/swagger�Ƿ��ʲ�����
                c.SwaggerEndpoint($"/swagger/V1/swagger.json", $"XUnit.Core V1");
                //·�����ã�����Ϊ�գ���ʾֱ���ڸ�������localhost:8001�����ʸ��ļ�
                c.RoutePrefix = "swagger"; // ������뻻һ��·����ֱ��д���ּ��ɣ�����ֱ��дc.RoutePrefix = "swagger"; �����·��Ϊ ������/swagger/index.html
            });

            app.UseRouting();

            //�������п���cors����ConfigureServices���������õĿ����������
            //ע�⣺UseCors�������UseRouting��UseEndpoints֮��
            app.UseCors("cors");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //���������RequireCors������cors����ConfigureServices���������õĿ����������
                endpoints.MapControllers().RequireCors("cors");
            });
        }
    }
}
