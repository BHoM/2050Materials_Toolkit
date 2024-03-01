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

namespace BH.Adapter.Materials2050
{
    public partial class Materials2050Adapter : BHoMAdapter
    {
        /***************************************************/
        /**** Adapter overload method                   ****/
        /***************************************************/
        protected override IEnumerable<IBHoMObject> IRead(Type type, IList ids, ActionConfig actionConfig = null)
        {
            dynamic elems = null;
            Materials2050Config config = null;

            if (actionConfig is Materials2050Config)
                config = actionConfig as Materials2050Config;

            if (type == typeof(EnvironmentalProductDeclaration))
                elems = ReadEnvironmentalProductDeclaration(ids as dynamic, config);

            return elems;
        }
        /***************************************************/
        /**** Private specific read methods             ****/
        /***************************************************/
        private List<CustomObject> ReadEnvironmentalProductDeclaration(List<string> ids = null, Materials2050Config config = null)
        {
            int count = config.Count;
            string name = config.NameLike;

            string refreshedToken = BH.Engine.Adapters.Materials2050.Create.RefreshAPIToken(m_apiToken);

            //Create GET Request
            GetRequest epdGetRequest;

            //Pass the refreshed token to the subsequent API calls
            epdGetRequest = BH.Engine.Adapters.Materials2050.Create.Materials2050Request("get_products_open_api", refreshedToken, config);


            string reqString = epdGetRequest.ToUrlString();
            string response = BH.Engine.Adapters.HTTP.Compute.MakeRequest(epdGetRequest);
            List<CustomObject> responseObjs = new List<CustomObject>();

            if (response == null)
            {
                BH.Engine.Base.Compute.RecordWarning("No response received, API token and connection.");
                return null;
            }
            //Check if the response is a valid json
            else if (response.StartsWith("{"))
            {
                response = "[" + response + "]";
                responseObjs.AddRange(Engine.Serialiser.Convert.FromJsonArray(response) as List<CustomObject>);
            }
            else if (response.StartsWith("["))
            {
                responseObjs.AddRange(Engine.Serialiser.Convert.FromJsonArray(response) as List<CustomObject>);
            }
            else
            {
                BH.Engine.Base.Compute.RecordWarning("Response is not a valid JSON. How'd that happen?");
                return null;
            }

            return responseObjs;

            ////Convert nested customObject from serialization to list of epdData objects
            //List<EnvironmentalProductDeclaration> epdDataFromRequest = new List<EnvironmentalProductDeclaration>();
            //object epdObjects = responseObjs[0];
            //IEnumerable objList = epdObjects as IEnumerable;
            //if (objList != null)
            //{
            //    foreach (CustomObject co in objList)
            //    {
            //        EnvironmentalProductDeclaration epdData = Adapter.Materials2050.Convert.ToEnvironmentalProductDeclaration(co, config);
            //        epdDataFromRequest.Add(epdData);
            //    }
            //}
            //return epdDataFromRequest;
        }

        /***************************************************/
    }
}

