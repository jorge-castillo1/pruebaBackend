﻿using customerportalapi.Entities;
using customerportalapi.Entities.enums;
using System;
using System.Reflection;

namespace customerportalapi.Services
{
    public static class UserInvitationUtils
    {
        public static string GetLanguage(string invitationLanguage)
        {
            switch (invitationLanguage.ToLower())
            {
                case "spanish":
                    return LanguageTypes.es.ToString();
                case "english":
                    return LanguageTypes.en.ToString();
                case "portuguese":
                    return LanguageTypes.pt.ToString();
                case "french":
                    return LanguageTypes.fr.ToString();
                default:
                    return LanguageTypes.en.ToString();
            }
        }

        public static int GetUserType(string invitationCustomerType)
        {
            switch (invitationCustomerType.ToLower())
            {
                case "residential":
                    return (int)UserTypes.Residential;
                case "business":
                    return (int)UserTypes.Business;
                default:
                    return (int)UserTypes.Residential;
            }
        }

        public static string GetAccountType(int userType)
        {
            switch (userType)
            {
                case (int)UserTypes.Residential:
                    return AccountType.Residential;
                case (int)UserTypes.Business:
                    return AccountType.Business;
                default:
                    return AccountType.Residential;
            }
        }

        public static InvitationMandatoryData InitInvitationData()
        {
            InvitationMandatoryData data = new InvitationMandatoryData
            {
                ContactUsername = GetMandatoryData(SystemTypes.CRM, EntityNames.contacts, null, StateEnum.Unchecked),
                Contract = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_contracts, null, StateEnum.Unchecked),
                SmContractCode = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_contracts, null, StateEnum.Unchecked),
                SMContract = GetMandatoryData(SystemTypes.SM, EntityNames.WBSGetContract, null, StateEnum.Unchecked),
                ActiveContract = GetMandatoryData(SystemTypes.SM, EntityNames.WBSGetContract, null, StateEnum.Unchecked),
                //Access Code eliminado temporalmente de Mandatory Data
                //UnitPassword = GetMandatiryData(SystemTypes.CRM, EntityNames.WBSGetContract, null, StateEnum.Unchecked),
                UnitName = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_contracts, null, StateEnum.Unchecked),
                UnitSizeCode = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_contracts, null, StateEnum.Unchecked), // TODO: check EntityNames
                ContractStoreCode = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_contracts, null, StateEnum.Unchecked),
                StoreCode = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_stores, null, StateEnum.Unchecked),
                OpeningDaysFirst = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_stores, null, StateEnum.Unchecked),
                OpeningDaysLast = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_stores, null, StateEnum.Unchecked),
                OpeningHoursFrom = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_stores, null, StateEnum.Unchecked),
                OpeningHoursTo = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_stores, null, StateEnum.Unchecked),
                StoreName = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_stores, null, StateEnum.Unchecked),
                StoreEmail = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_stores, null, StateEnum.Unchecked),
                StoreCity = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_stores, null, StateEnum.Unchecked),
                ContractOpportunity = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_contracts, null, StateEnum.Unchecked),
                OpportunityId = GetMandatoryData(SystemTypes.CRM, EntityNames.opportunities, null, StateEnum.Unchecked),
                ExpectedMoveIn = GetMandatoryData(SystemTypes.CRM, EntityNames.opportunities, null, StateEnum.Unchecked),

                SiteMailType = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_stores, ((int)StoreMailTypes.WithoutSignageOrNull).ToString(), StateEnum.Checked),
                UnitColour = GetMandatoryData(SystemTypes.CRM, EntityNames.products, null, StateEnum.Checked),
                UnitCorridor = GetMandatoryData(SystemTypes.CRM, EntityNames.products, null, StateEnum.Checked),
                UnitExceptions = GetMandatoryData(SystemTypes.CRM, EntityNames.products, null, StateEnum.Checked),
                UnitFloor = GetMandatoryData(SystemTypes.CRM, EntityNames.products, null, StateEnum.Checked),
                UnitZone = GetMandatoryData(SystemTypes.CRM, EntityNames.products, null, StateEnum.Checked),

                InvokedBy = GetMandatoryData(SystemTypes.empty, EntityNames.empty, null, StateEnum.Unchecked),
            };

            return data;
        }

        public static MandatoryData GetMandatoryData(SystemTypes system, EntityNames entity, string value, StateEnum state)
        {
            MandatoryData data = new MandatoryData()
            {
                Value = value,
                State = state,
                System = system,
                Entity = entity
            };

            return data;
        }


        public static string GetBodyFormatted(EmailTemplate invitationTemplate, User user, InvitationMandatoryData fields, string baseURL, string inviteConfirmation)
        {
            string body = invitationTemplate.body;

            PropertyInfo[] properties = typeof(InvitationMandatoryData).GetProperties();
            foreach (var property in properties)
            {
                MandatoryData data = (MandatoryData)property.GetValue(fields);
                if (data != null && !string.IsNullOrEmpty(data.Value))
                {
                    string field = "{{" + property.Name + "}}";

                    switch (property.Name)
                    {
                        case "ExpectedMoveIn":
                            DateTime expectedMoveIn = DateTime.Parse(data.Value);
                            body = body.Replace(field, expectedMoveIn.ToString("dd/MM/yyyy"));
                            expectedMoveIn = expectedMoveIn.Subtract(TimeSpan.FromDays(1));
                            body = body.Replace("{{PreviousExpectedMoveIn}}", expectedMoveIn.ToString("dd/MM/yyyy"));
                            break;

                        case "ContactUsername":
                            body = body.Replace(field, data.Value);
                            body = body.Replace("{{UserPassword}}", user.Password);
                            break;

                        case "UnitName":
                            body = body.Replace(field, data.Value);
                            char[] unitName = data.Value.ToCharArray();

                            int num;
                            if (unitName[0].ToString() != null && int.TryParse(unitName[0].ToString(), out num))
                            {
                                num++;
                                if (num > 9) num = 0;
                                unitName[0] = Char.Parse(num.ToString());
                            }

                            if (unitName[3].ToString() != null && int.TryParse(unitName[3].ToString(), out num))
                            {
                                num++;
                                if (num > 9) num = 0;
                                unitName[3] = Char.Parse(num.ToString());
                            }

                            body = body.Replace("{{LockCode}}", new string(unitName));
                            break;

                        //Access Code eliminado temporalmente de Mandatory Data
                        //case "UnitPassword":
                        case "UnitSizeCode":
                            var content = string.Empty;
                            if (invitationTemplate != null && invitationTemplate.Paragraphs != null)
                            {
                                EmailParagraph paragraph = GetParagraphByName(invitationTemplate, property.Name);
                                content = paragraph.CustomContent;
                                if (data.State == StateEnum.Checked)
                                {
                                    content = paragraph.DefaultContent;
                                    content = content.Replace(field, data.Value);
                                }
                            }

                            body = body.Replace("{{Paragraph" + property.Name + "}}", content);
                            break;

                        case "InvokedBy":
                            if (data.Value == ((int)InviteInvocationType.CronJob).ToString())
                            {
                                // only remove magic words
                                body = body.Replace("{{RememberSTART}}", string.Empty)
                                    .Replace("{{RememberEND}}", string.Empty);
                            }
                            else
                            {
                                // Remove all characters between the starting string and the ending string
                                body = RemoveString(body, "{{RememberSTART}}", "{{RememberEND}}");
                            }
                            break;

                        case "SiteMailType":
                            var intSiteMailType = (int)StoreMailTypes.WithoutSignageOrNull;
                            if (!string.IsNullOrEmpty(data.Value))
                                intSiteMailType = Convert.ToInt32(data.Value);
                            switch (intSiteMailType)
                            {
                                case (int)StoreMailTypes.NewSignage:
                                    body = body.Replace("{{Planta}}", fields.UnitFloor.Value)
                                            .Replace("{{Zona}}", fields.UnitZone.Value)
                                            .Replace("{{ColorZona}}", fields.UnitColour.Value)
                                            .Replace("{{Pasillo}}", fields.UnitCorridor.Value)
                                            .Replace("{{Excepciones}}", fields.UnitExceptions.Value)
                                            .Replace("{{LocationSTART}}", string.Empty)
                                            .Replace("{{LocationEND}}", string.Empty);
                                    break;

                                case (int)StoreMailTypes.OldSignage:
                                    body = body.Replace("{{Excepciones}}", fields.UnitExceptions.Value);
                                    body = RemoveString(body, "{{LocationSTART}}", "{{LocationEND}}");
                                    break;

                                case (int)StoreMailTypes.WithoutSignageOrNull:
                                default:
                                    body = body.Replace("{{Excepciones}}", string.Empty);
                                    body = RemoveString(body, "{{LocationSTART}}", "{{LocationEND}}");
                                    break;
                            }
                            break;

                        default:
                            body = body.Replace(field, data.Value);
                            break;
                    }
                }
            }
            body = body.Replace("{{BaseUrl}}", baseURL);
            body = body.Replace("{{InviteConfirmationUrl}}", inviteConfirmation + user.Invitationtoken);

            return body;
        }

        private static void OldSignageFormat(ref string body, string unitExceptions)
        {
            body = body.Replace("{{Excepciones}}", unitExceptions);
        }

        private static void NewSignageFormat(ref string body, string unitColour, string unitCorridor, string unitExceptions, string unitFloor, string unitZone)
        {
            body = body
                .Replace("{{Planta}}", unitFloor)
                .Replace("{{Zona}}", unitZone)
                .Replace("{{Color de la zona}}", unitColour)
                .Replace("{{Excepciones}}", unitExceptions);
        }

        private static EmailParagraph GetParagraphByName(EmailTemplate template, string name)
        {
            name = name.ToLower();
            if (template != null && template.Paragraphs.Count > 0)
            {
                return template.Paragraphs.Find((t) => t.Name == name);
            }

            return null;
        }

        /// <summary>
        /// Remove all characters between the starting string and the ending string (both included)
        /// </summary>
        /// <param name="source">String</param>
        /// <param name="start">Starting string</param>
        /// <param name="end">Ending string</param>
        /// <returns>Returns the string without the characters between "start" and "end"</returns>
        public static string RemoveString(string source, string start, string end)
        {
            string result = source;
            if (source.Contains(start) && source.Contains(end))
            {
                int startIndex = source.IndexOf(start);
                int endIndex = source.IndexOf(end, startIndex) + end.Length;
                //result = source.Substring(startIndex, endIndex - startIndex);
                result = source.Remove(startIndex, endIndex - startIndex);
                return result;
            }

            return result;
        }
    }
}
