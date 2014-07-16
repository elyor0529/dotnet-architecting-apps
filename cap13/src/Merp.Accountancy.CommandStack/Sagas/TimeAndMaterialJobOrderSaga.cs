﻿using Merp.Accountancy.CommandStack.Commands;
using Merp.Accountancy.CommandStack.Events;
using Merp.Accountancy.CommandStack.Model;
using Merp.Accountancy.CommandStack.Services;
using Merp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Accountancy.CommandStack.Sagas
{
    public class TimeAndMaterialJobOrderSaga : Saga,
        IAmStartedBy<CreateTimeAndMaterialJobOrderCommand>
    {
        public IJobOrderNumberGenerator JobOrderNumberGenerator { get; private set; }

        public TimeAndMaterialJobOrderSaga(IBus bus, IEventStore eventStore, IRepository repository, IJobOrderNumberGenerator jobOrderNumberGenerator)
            : base(bus, eventStore, repository)
        {
            if(jobOrderNumberGenerator==null)
            {
                throw new ArgumentNullException("jobOrderNumberGenerator");
            }
            JobOrderNumberGenerator = jobOrderNumberGenerator;
        }

        public void Handle(CreateTimeAndMaterialJobOrderCommand message)
        {
            var jobOrder = TimeAndMaterialJobOrder.Factory.CreateNewInstance(
                JobOrderNumberGenerator,
                message.CustomerId,
                message.Value,
                message.DateOfStart,
                message.DateOfExpiration,
                message.JobOrderName
                );
            this.Repository.Save(jobOrder);
            var @event = new TimeAndMaterialJobOrderCreatedEvent(
                jobOrder.Id,
                jobOrder.CustomerId,
                jobOrder.Value,
                jobOrder.DateOfStart,
                jobOrder.DateOfExpiration,
                jobOrder.Name,
                jobOrder.Number
                );
            Bus.RaiseEvent(@event);
        }
    }
}