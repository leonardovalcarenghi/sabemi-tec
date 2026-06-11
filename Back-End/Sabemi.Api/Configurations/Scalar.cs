using Scalar.AspNetCore;

namespace Sabemi.Api.Configurations;

public static class Scalar
{
    public static void AddScalar(this WebApplication app)
    {
        app.MapOpenApi();
        app.UseStaticFiles();
        app.MapScalarApiReference("/docs", options =>
        {
            options.Authentication = new ScalarAuthenticationOptions
            {
                PreferredSecuritySchemes = ["Bearer"]
            };

            options
                .WithOpenApiRoutePattern("/openapi/v1.json")
                .WithTitle("Sabemi API - Scalar")
                .WithCustomCss(CSS_CONTENT)
                .AddHeaderContent(HEADER_CONTENT)
                .SortTagsAlphabetically();
        });
    }

    private const string HEADER_CONTENT = @"
        <header class='header scalar-app'>
            <nav>
                <div>
                    <h3>
                        Sabemi API - Scalar
                    </h3>
                    <span>
                        Desenvolvido por Leonardo Valcarenghi
                    </span>
                </div>
                <div>
                    <a href='/hangfire' target='_blank'>
                        Ir para o Dashboard do Hangfire
                    </a>
                </div>
            </nav>
        </header>
    ";

    private const string CSS_CONTENT = @"

        :root {
            --scalar-custom-header-height: 50px;
            --scalar-font: ""Saira"", Sans-serif;
        }

        .header {       
            display: grid;
            align-items: center;
            padding: 0 20px;
            height: var(--scalar-custom-header-height);
            background-color: #2F2852;
        }

        .header * {
            color: #fff;
            margin: 0;
        }

        .header h3{ font-size: 15px }
        .header span{ font-size: 12px }

        .header nav{
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 18px;
        }

    ";
}
