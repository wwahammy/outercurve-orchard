var LeadersModel = function(multipleLeaders) {
    var self = this;
    self.multipleLeaders = ko.observableArray(ko.utils.arrayMap(multipleLeaders, function(leader) {
        return { id: leader.Id };
    }));

    self.addLeader = function() {
        self.multipleLeaders.push({
            id: ""
        });
    };

    self.removeLeader = function(leader) {
        self.multipleLeaders.remove(leader);
    };
}