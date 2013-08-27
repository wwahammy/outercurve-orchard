var ViewModel = function(signed, foundationSigner, employerSigner, signedByCompany) {
        this.isSigned = ko.observable(signed);
        this.hasFoundationSigner = ko.observable(foundationSigner);
        this.hasEmployerSigner = ko.observable(employerSigner);
        this.signedByCompany = ko.observable(signedByCompany);
};
