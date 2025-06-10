global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Diagnostics;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.Extensions.Options;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.Extensions.Caching.Hybrid;
global using Microsoft.AspNetCore.WebUtilities;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using Microsoft.AspNetCore.RateLimiting;
global using Microsoft.EntityFrameworkCore.Query;
global using Microsoft.EntityFrameworkCore.Diagnostics;
global using Microsoft.AspNetCore.SignalR;

global using System.Text.Json;
global using System.Linq.Dynamic.Core;
global using System.ComponentModel.DataAnnotations;
global using System.Threading.RateLimiting;
global using System.Security.Cryptography;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using System.Reflection;
global using System.Text;

global using AutoMapper;

global using HealthChecks.UI.Client;

global using Hangfire;
global using HangfireBasicAuthenticationFilter;

global using MimeKit;
global using MailKit.Net.Smtp;
global using MailKit.Security;

global using FluentValidation;

global using Serilog;

global using SyriaNews.Models;
global using SyriaNews.Health;
global using SyriaNews.DTOs.Common;
global using SyriaNews.Abstractions.Consts;
global using SyriaNews.DTOs.Role;
global using SyriaNews.DTOs.Notification;
global using SyriaNews.DTOs.ArticleTag;
global using SyriaNews.DTOs.Comment;
global using SyriaNews.DTOs.Follower;
global using SyriaNews.DTOs.Admin;
global using SyriaNews.DTOs.ImageAndProfile;
global using SyriaNews.DTOs.Article;
global using SyriaNews.DTOs.Member;
global using SyriaNews.DTOs.Like;
global using SyriaNews.DTOs.Save;
global using SyriaNews.DTOs.NewsPaper;
global using SyriaNews.Repository.Implementations;
global using SyriaNews.ResultsExtension;
global using SyriaNews.UnitOfWork;
global using SyriaNews.Abstractions;
global using SyriaNews.Models;
global using SyriaNews.Enums;
global using SyriaNews.Repository.Interfaces;
global using SyriaNews.Errors;
global using SyriaNews.DTOs.Auth;
global using SyriaNews.DTOs.Category;
global using SyriaNews.Data;
global using SyriaNews.AutoMapper;
global using SyriaNews.ExceptionHandler;
global using SyriaNews.HelperTools;
global using SyriaNews.DTOs.Tags;
