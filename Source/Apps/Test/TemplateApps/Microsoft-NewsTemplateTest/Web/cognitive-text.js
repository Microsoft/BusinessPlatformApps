"use strict";
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var viewmodelbase_1 = require("../../../../../SiteCommon/Web/services/viewmodelbase");
var CognitiveText = (function (_super) {
    __extends(CognitiveText, _super);
    function CognitiveText() {
        var _this = _super.call(this) || this;
        _this.isBingChecked = false;
        _this.isValidated = false;
        return _this;
    }
    CognitiveText.prototype.verifyBing = function () {
        this.isValidated = this.isBingChecked;
    };
    return CognitiveText;
}(viewmodelbase_1.ViewModelBase));
exports.CognitiveText = CognitiveText;
//# sourceMappingURL=cognitive-text.js.map