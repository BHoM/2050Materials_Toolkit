/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using BH.Engine.Adapters.HTTP;
using BH.oM.Adapter;
using BH.oM.Adapters.Materials2050;
using BH.Engine.Adapters.Materials2050;
using BH.oM.Adapters.HTTP;
using BH.oM.Base;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using System;
using System.Collections;
using System.Collections.Generic;
using BH.Engine.Base;
using BH.Engine.Reflection;
using BH.oM.LifeCycleAssessment;
using BH.oM.Materials2050;
using System.Linq;

namespace BH.Adapter.Materials2050
{
    public partial class Materials2050Adapter : BHoMAdapter
    {
        /***************************************************/
        /**** Adapter overload method                   ****/
        /***************************************************/
        protected override IEnumerable<IBHoMObject> IRead(Type type, IList ids, ActionConfig actionConfig = null)
        {
            List<EnvironmentalProductDeclaration> elems = new List<EnvironmentalProductDeclaration>();
            Materials2050Config config = null;

            if (actionConfig is Materials2050Config)
                config = actionConfig as Materials2050Config;

            if (type == typeof(EnvironmentalProductDeclaration))
                elems = ReadEnvironmentalProductDeclaration(config);

            return elems;
        }
        /***************************************************/
        /**** Private specific read methods             ****/
        /***************************************************/

        private List<EnvironmentalProductDeclaration> ReadEnvironmentalProductDeclaration(Materials2050Config config = null)
        {
            // Determine the API type to be used for the request
            string apiName = "";
            if(config != null)
            {
                Enum apiT = config.APIName;
                switch (apiT)
                {
                    case APIName.OpenAPI:
                        {
                            apiName = "get_products_open_api";
                            BH.Engine.Base.Compute.RecordNote($"Using {apiT} for all queries. All metrics will be reported as A3 ClimateChangeTotalMetrics.");
                            break;
                        }
                    case APIName.GenericMaterialsAPI:
                        {
                            apiName = "get_generic_materials";
                            BH.Engine.Base.Compute.RecordError($"The {apiT} API has not yet been implemented. Please use the OpenAPI.");
                            break;
                        }
                    case APIName.ProductAPI:
                        {
                            apiName = "get_products";
                            BH.Engine.Base.Compute.RecordError($"The {apiT} API has not yet been implemented. Please use the OpenAPI.");
                            break;
                        }
                    case APIName.GlobalWarmingAPI:
                        {
                            apiName = "get_co2_warming_potential";
                            BH.Engine.Base.Compute.RecordError($"The {apiT} API has not yet been implemented. Please use the OpenAPI.");
                            break;
                        }
                    default:
                        apiName = "";
                        BH.Engine.Base.Compute.RecordError("Please select which API you would like to query.");
                        break;
                }
            }
            else
            {
                BH.Engine.Base.Compute.RecordWarning("No config was provided. Using OpenAPI as the default 2050 Materials API for all queries.");
                apiName = "get_products_open_api";
            }

            // Refresh the API Token automatically
            string refreshedToken = BH.Engine.Adapters.Materials2050.Create.RefreshAPIToken(m_apiToken);

            //Create GET Request
            GetRequest epdGetRequest = BH.Engine.Adapters.Materials2050.Create.Materials2050Request(apiName, refreshedToken, config);

            string response = BH.Engine.Adapters.HTTP.Compute.MakeRequest(epdGetRequest);
            List<object> responseObjs = new List<object>();

            if (string.IsNullOrEmpty(response))
            {
                BH.Engine.Base.Compute.RecordWarning("No response received, API token and connection.");
                return null;
            }
            //Check if the response is a valid json
            else if (response.StartsWith("{"))
            {
                response = "[" + response + "]";
                responseObjs = new List<object>() { Engine.Serialiser.Convert.FromJsonArray(response) };
            }
            else if (response.StartsWith("["))
            {
                responseObjs = new List<object>() { Engine.Serialiser.Convert.FromJsonArray(response) };
            }
            else
            {
                BH.Engine.Base.Compute.RecordWarning("Response is not a valid JSON. Please check your config and try again.");
                return null;
            }

            //Convert nested customObject from serialization to list of epdData objects
            List<EnvironmentalProductDeclaration> epds = new List<EnvironmentalProductDeclaration>();
            EnvironmentalProductDeclaration epd = new EnvironmentalProductDeclaration();
            
            if(responseObjs.Count >= 0) 
            {
                object epdObjects = responseObjs[0]; // we always want the zero index item. In the event it's the wrong data, other errors will be triggered.
                IEnumerable objList = epdObjects as IEnumerable;

                if (objList != null)
                {
                    foreach (CustomObject co in objList)
                    {
                        List<CustomObject> resultsObjs = new List<CustomObject>();
                        try
                        {
                            resultsObjs = (co.CustomData["results"] as List<object>).Cast<CustomObject>().ToList();

                            foreach (CustomObject co2 in resultsObjs)
                            {
                                epds.Add(Convert.ToEnvironmentalProductDeclaration(co, co2, config));
                            }
                        }
                        catch (Exception ex)
                        {
                            BH.Engine.Base.Compute.RecordError(ex, "No results were found from the pulled data.");
                            return null;
                        }
                    }
                }
                BH.Engine.Base.Compute.RecordNote($"{epds.Count} EPDs have been converted from the 2050 Materials {apiName} request.");
                return epds;

            } else
            {
                BH.Engine.Base.Compute.RecordError("No data was received from the API call");
                return null;
            }
        }

        /***************************************************/
    }
}

