using MediatR;
using Pos.Modules.Payments.Application.DTOs;
using Pos.Shared.Common;

namespace Pos.Modules.Payments.Application.Queries;

public record GetAllPaymentsQuery(Guid? OrderId = null) : IRequest<Result<IEnumerable<PaymentDto>>>;
public record GetPaymentByIdQuery(Guid Id) : IRequest<Result<PaymentDto>>;

