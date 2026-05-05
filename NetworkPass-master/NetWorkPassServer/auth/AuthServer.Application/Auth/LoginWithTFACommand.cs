using AuthServer.Application.Dtos;
using AuthServer.Application.Services;
using AuthServer.Domain.Users;
using GenericRepository;
using SharedLibrary;
using System.Net;
using TS.MediatR;

namespace AuthServer.Application.Auth;
public sealed record LoginWithTFACommand(string UserName, string TFACode) : IRequest<ServiceResult<LoginResult>>;



public sealed class LoginWithTFACommandHandler(IUserRepository userRepository, ITokenService tokenService, IUnitOfWork unitOfWork) : IRequestHandler<LoginWithTFACommand, ServiceResult<LoginResult>>

{
    public async Task<ServiceResult<LoginResult>> Handle(LoginWithTFACommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FirstOrDefaultAsync(
            x => x.Email.Value == request.UserName || x.UserName.Value == request.UserName,
            cancellationToken
        );

        if (user is null)
            return ServiceResult<LoginResult>.Failure(
                "Authentication Failed",
                "İstifadəçi adı və ya şifrə düzgün deyil.",
                HttpStatusCode.Unauthorized
            );

        if (user.TFAIsCompleted is null || user.TFAExpiresDate is null || user.TFACode is null)
            return ServiceResult<LoginResult>.Failure(
                "TFA Error",
                "TFA Kodu etibarsızdır.",
                HttpStatusCode.BadRequest
            );

        if (user.TFAIsCompleted.Value)
            return ServiceResult<LoginResult>.Failure(
                "TFA Error",
                "TFA Kodu artıq istifadə edilib.",
                HttpStatusCode.BadRequest
            );

        if (user.TFAExpiresDate.Value < DateTimeOffset.Now)
            return ServiceResult<LoginResult>.Failure(
                "TFA Error",
                "TFA Kodu vaxtı keçmişdir.",
                HttpStatusCode.BadRequest
            );

        if (user.TFACode.Value != request.TFACode)
            return ServiceResult<LoginResult>.Failure(
                "TFA Error",
                "TFA Kodu düzgün deyil.",
                HttpStatusCode.BadRequest
            );
        //TFA tamamlandı
        user.SetTFACompleted();
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var token = await tokenService.GenerateTokenAsync(user, cancellationToken);
        var res = new LoginResult
        {
            
            RequiresTFA = false,
            Token= token,
        };

        return ServiceResult<LoginResult>.Success(res) ;
    }
}