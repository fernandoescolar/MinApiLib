global using Example.Data;

global using Microsoft.EntityFrameworkCore;
global using Microsoft.OpenApi.Models;

global using MinApiLib.DependencyInjection;
global using MinApiLib.Endpoints;
global using MinApiLib.HashedIds;
global using MinApiLib.Hypermedia;
global using MinApiLib.OperationCanceled;
global using MinApiLib.Validation;

global using Example.Simple.Api;
global using Example.Simple.Api.Beers.Models;
global using Example.Simple.Api.Beers.Hypermedia;
global using Example.Simple.Api.BeerStyles.Models;
global using Example.Simple.Api.BeerStyles.Hypermedia;
global using Example.Simple.Api.Breweries.Models;
global using Example.Simple.Api.Breweries.Hypermedia;

global using System.ComponentModel.DataAnnotations;

global using Endpoint = MinApiLib.Endpoints.Endpoint;
