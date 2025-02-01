<script setup>
import AddUser from "./Tabs/Admin/AddUser.vue";
import EditTable from "./Tabs/Admin/EditTable.vue";
import ServerContent from "./Tabs/Admin/ServerContent.vue";
</script>
<template>
  <div class="tabs">
    <button @click="ChangeTab(0)" :class="{ activebc: Tab === 0 }">Admins</button>
    <button @click="ChangeTab(1)" :class="{ activebc: Tab === 1 }">Server Management</button>
    <button disabled>Coming Soon</button>
  </div>

  <div v-if="Tab == 0">
    <div v-if="!ShowEditAdmin" class="table-container">
      <button v-if="UserAdmin" @click="AddNewUser">Add</button>
      <table>
        <thead>
          <tr class="header-row">
            <th>Section</th>
            <th>Body</th>
            <th>Action</th>
          </tr>
        </thead>
        <tbody>
          <tr class="row">
            <td>Admin</td>
            <td>
              The Admin Account Is Forced (you cannot edit or delete this)
            </td>
            <td>
              <button disabled>Edit</button>
            </td>
          </tr>
          <tr v-for="(editthing, index) in sections" :key="index" class="row">
            <td>{{ editthing.Username }}</td>
            <td>{{ editthing.Message }}</td>
            <td>
              <button
                type="text"
                @click="EditUser(index)"
                :disabled="editthing.Disabled"
              >
                Edit
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-if="ShowEditAdmin && !ShowAddUser">
      <EditTable
        @back="backfr"
        :DATASENT="DATASENT"
        @updatedData="UpdatedDATA"
      />
    </div>

    <div v-if="!ShowEditAdmin && ShowAddUser">
      <AddUser @back="backfr" />
    </div>
  </div>

  <div v-if="Tab == 1">
     <ServerContent />
  </div>
</template>

<script>
export default {
  data() {
    return {
      sections: [],
      PanelData: Object,
      DATASENT: Object,
      UserAdmin: false,
      ShowEditAdmin: false,
      ShowAddUser: false,
      Tab: 0,
    };
  },
  methods: {
    ChangeTab(TabID) {
       if(this.Tab != TabID) {
          this.Tab = TabID;
       }
    },
    async EditUser(ggs) {
      // since we already got data just like show it already.....
      console.log(this.PanelData.AdminLists[ggs]);
      //this.DATASENT = this.PanelData.AdminLists[ggs];
      // nearly forgot about this endpoint! ~ this is called on every user ~ if they lost permission they shouldnt have access
      const apiUrl = import.meta.env.VITE_API_URL;
      const apiResponse = await fetch(
        `${apiUrl}/admin/new/dashboard/panel/user/edit/${this.PanelData.AdminLists[ggs].AccountId}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          credentials: "include",
        }
      );
      var responsefr = await apiResponse.json();
      if (responsefr) {
        console.log(responsefr);
        this.DATASENT = responsefr;
        this.ShowEditAdmin = true;
      }
    },
    AddNewUser() {
      this.ShowAddUser = true;
    },
    backfr() {
      this.ShowEditAdmin = false;
      this.ShowAddUser = false;
    },
    UpdatedDATA(data) {
      console.log("UPDATED DATA " + data);
      var test = [];

      var test = this.PanelData.AdminLists.findIndex(
        (e) => e.AccountId == data.AccountId
      );
      if (test != -1) {
        // need to check if the user demoted it self...
        // kinda stupid but we remove and add back
        this.PanelData.AdminLists[test].adminInfo.Role = data.adminInfo.Role;
        console.log(this.PanelData.AdminLists[test].adminInfo.Role);
        var frfr = this.sections[0].Disabled;
        this.sections = [];
        this.PanelData.AdminLists.forEach((e) => {
          console.log(e);
          this.sections.push({
            Username: e.DATA.UserName,
            Message:
              e.adminInfo.Role == 1
                ? "Moderators cannot 'add' / 'edit' users"
                : "N/A",
            Disabled: frfr,
          });
        });
      }
    },
  },
  async mounted() {
    const apiUrl = import.meta.env.VITE_API_URL;
    const apiResponse = await fetch(`${apiUrl}/admin/new/dashboard/panel`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      credentials: "include",
    });
    var responsefr = await apiResponse.json();
    console.log(responsefr);
    this.PanelData = responsefr;
    this.UserAdmin = responsefr.admin;
    responsefr.AdminLists.forEach((e) => {
      console.log(e.DATA.UserName);
      this.sections.push({
        Username: e.DATA.UserName,
        Message:
          e.adminInfo.Role == 1
            ? "Moderators cannot 'add' / 'edit' users"
            : "N/A",
        Disabled: !responsefr.admin,
      });
    });
  },
};
</script>

<style scoped>
.tabs {
  display: flex;
  gap: 10px;
  margin-bottom: 10px;
}
.table-container {
  overflow: hidden;
}

table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
}

thead {
  background-color: #424242;
}

.header-row th:first-child {
  border-top-left-radius: 10px;
  border-bottom-left-radius: 10px;
}

.header-row th:last-child {
  border-top-right-radius: 10px;
  border-bottom-right-radius: 10px;
}

.row {
  border-bottom: 3px double #ddd;
}

td,
th {
  padding: 12px;
  text-align: left;
}

.activebc {
  background-color: rgb(255, 255, 255);
  color: black
}
</style>
