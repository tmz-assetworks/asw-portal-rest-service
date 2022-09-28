using Newtonsoft.Json;
using PortalRestService.Application;
using PortalRestService.Core.Entities.Charger;
using PortalRestService.Core.Repositories;
using PortalRestService.Core.Responses;
using PortalRestService.Helper;
using PortalRestService.Infrastructure.Repositories.Repository;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using PortalRestService.Helpers;
using PortalRestService.Infrastructure.Helper;

namespace PortalRestService.Infrastructure.Repositories.Assets
{
#pragma warning disable
    public class RfIdReaderRepository : Repository<RfIdReaderDetailsResponse>, IRfIdReaderRepository
    {
        TokenBase _tokenBase;
        public RfIdReaderRepository(TokenBase token) : base()
        {
            _tokenBase=token;
        }
       public async Task<RfIdReaderDetailsResponse> GetRfIdReaderById(long  Id)
        {
            RFIDReaderDetails rfIDReaderDetails = new RFIDReaderDetails();
            string callingMethod = APIConstant.GetRfIdReaderById;
            RfIdReaderDetailsResponse dfIdReaderDetailsResponse = new RfIdReaderDetailsResponse();
            try
            {
                HttpResponseMessage response = await Helpers.Helper.GetCallAssetAuthAPIAsync(callingMethod,_tokenBase.acces_token);   // Returens Data with Pagination

                if (response.IsSuccessStatusCode)
                {
                    var dispenserdetails = await response.Content.ReadAsStringAsync();
                    dfIdReaderDetailsResponse = JsonConvert.DeserializeObject<RfIdReaderDetailsResponse>(dispenserdetails);
                    if (dfIdReaderDetailsResponse != null && dfIdReaderDetailsResponse.data != null )
                        dfIdReaderDetailsResponse.StatusMessage = "Record found.";
                    else dfIdReaderDetailsResponse.StatusMessage = "Record not found.";
                    dfIdReaderDetailsResponse.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    dfIdReaderDetailsResponse.StatusMessage ="Internal server Error";
                }
            }
            catch (Exception ex)
            {
                dfIdReaderDetailsResponse.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            return dfIdReaderDetailsResponse;
        }

    }
}
