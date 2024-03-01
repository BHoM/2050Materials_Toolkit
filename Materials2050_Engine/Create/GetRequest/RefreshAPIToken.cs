/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System;
using System.Linq;
using BH.oM.Adapters.HTTP;
using BH.oM.Adapters.Materials2050;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using BH.Engine.Serialiser;
using BH.oM.Base;

namespace BH.Engine.Adapters.Materials2050
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public  Method                            ****/
        /***************************************************/

        [Description("Refresh the API Token for the 2050 Materials API queries")]
        [Input("apiToken", "The user's 2050 Materials APIToken")]
        [Output("NewAPIToken", "A GetRequest with 2050 Materials specific headers and uri")]
        public static string RefreshAPIToken(string apiToken)
        {
            if(apiToken!=null)
            {
                // GET 
                GetRequest request = new BH.oM.Adapters.HTTP.GetRequest
                {
                    BaseUrl = "https://app.2050-materials.com/developer/api/getapitoken/",
                    Headers = new Dictionary<string, object>()
                    {
                        { "Authorization", "Bearer " + apiToken }
                    },
                    Parameters = null
                };

                string response = BH.Engine.Adapters.HTTP.Compute.MakeRequest(request);

                if(string.IsNullOrEmpty(response))
                {
                    BH.Engine.Base.Compute.RecordError("No response received for API token, unable to continue.");
                    return null;
                }

                try
                {
                    CustomObject cObj = BH.Engine.Serialiser.Convert.FromJson(response) as CustomObject;
                    if (cObj.CustomData.ContainsKey("api_token"))
                        return cObj.CustomData["api_token"] as string;
                    else
                    {
                        BH.Engine.Base.Compute.RecordError($"Response {response} did not contain an API_TOKEN item to return.");
                        return null;
                    }
                }
                catch(Exception e)
                {
                    BH.Engine.Base.Compute.RecordError(e, $"Error while converting {response} to custom object.");
                    return null;
                }
            }
            else
            {
                BH.Engine.Base.Compute.RecordWarning("Please provide an API Token.");
                return null;
            }
        }
        /***************************************************/
    }
}



