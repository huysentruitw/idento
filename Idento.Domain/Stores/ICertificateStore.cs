/*
 * Copyright 2016-2017 Wouter Huysentruit
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Threading.Tasks;
using Idento.Domain.Entities;

namespace Idento.Domain.Stores
{
    public interface ICertificateStore
    {
        Task<Certificate[]> GetAll(Guid tenantId);
        Task<Certificate> FindById(Guid tenantId, Guid id);
        Task<Certificate> FindByName(Guid tenantId, string name);
        Task Create(Guid tenantId, Certificate certificate);
        Task Update(Guid tenantId, Guid id, Action<Certificate> updateAction);
        Task Delete(Guid tenantId, Guid id);
    }
}
