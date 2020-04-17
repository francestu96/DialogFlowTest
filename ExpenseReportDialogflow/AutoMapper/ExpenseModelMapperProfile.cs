
using AutoMapper;
using ExpenseReportDialogflow.Models;
using Google.Cloud.Dialogflow.V2;
using System.Collections.Generic;

namespace ExpenseReportDialogflow.AutoMapper
{
	public class ExpenseModelMapperProfile : Profile
	{
		public ExpenseModelMapperProfile()
		{
			CreateMap<DetectIntentResponse, ExpenseModel>()
				.ForMember(dest => dest.Place, opt => opt.MapFrom(source => source.QueryResult.Parameters.Fields.GetValueOrDefault("Place").StringValue))
				.ForMember(dest => dest.Price, opt => opt.MapFrom(source => source.QueryResult.Parameters.Fields.GetValueOrDefault("Price").NumberValue))
				.ForMember(dest => dest.ExpenseType, opt => opt.MapFrom(source => source.QueryResult.Parameters.Fields.GetValueOrDefault("ExpenseType").StringValue));
		}
	}
}
