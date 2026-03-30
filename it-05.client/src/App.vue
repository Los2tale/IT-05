<script setup>
  import 'bootstrap/dist/css/bootstrap.min.css';

  import { ref } from 'vue';

  import moment from 'moment'
  import QueueService from './services/QueueService.js';

  const Page1isVisible = ref(true);
  const Page2isVisible = ref(false);
  const Page3isVisible = ref(false);

  const data = ref(null);
  const isLoading = ref(true);
  const error = ref(null);

  const queue = ref("");
  const date = ref("");

  const createQueue = async () => {
    try {
      const response = await QueueService.post();
      if (response.status != 200) {
        throw new Error('Network response was not ok');
      }
      const result = await response.data;

      if (result.queueNumber == 0) {
        queue.value = "No Queue";
      } else {
        queue.value = result.queueNumber;
      }

      date.value = moment(String(result.createTime)).format('DD/MM/YYYY เวลา HH:mm น.');

    } catch (err) {
      error.value = err;
      console.error('Error fetching data:', err);
    } finally {
      isLoading.value = false;
      Page1isVisible.value = false;
      Page2isVisible.value = true;
      Page3isVisible.value = false;
    }
  };

  const backToPage1 = () => {
    Page1isVisible.value = true;
    Page2isVisible.value = false;
    Page3isVisible.value = false;
  };

  const goToPage3 = async () => {
    try {
      const response = await QueueService.get();
      if (response.status != 200) {
        throw new Error('Network response was not ok');
      }
      const result = await response.data;

      if (result.queueNumber == 0) {
        queue.value = "No Queue";
      } else {
        queue.value = result.queueNumber;
      }
      date.value = moment(String(result.createTime)).format('DD/MM/YYYY เวลา HH:mm น.');

    } catch (err) {
      error.value = err;
      console.error('Error fetching data:', err);
    } finally {
      isLoading.value = false;
      Page1isVisible.value = false;
      Page2isVisible.value = false;
      Page3isVisible.value = true;
    }
    
  };

  const clearQueue = async () => {
    try {
      const response = await QueueService.delete();
      if (response.status != 200) {
        throw new Error('Network response was not ok');
      }
      const result = await response.data;

      queue.value = result.queueNumber;
      date.value = moment(String(result.createTime)).format('DD/MM/YYYY เวลา HH:mm น.');

    } catch (err) {
      error.value = err;
      console.error('Error fetching data:', err);
    } finally {
      isLoading.value = false;
      Page1isVisible.value = false;
      Page2isVisible.value = false;
      Page3isVisible.value = true;
    }

  };

</script>

<template>
  <div class="container text-center">
    <div class="col">
      <div class="row">
        <div class="col">
          <header>
            <div>
              <h1>ระบบรับบัตรคิว</h1>
            </div>
          </header>
        </div>
      </div>
      <div class="row">
        <main>

          <div v-show="Page1isVisible">
            <div>
              <input id="GetQueueButton" type="button" value="รับบัตรคิว" @click="createQueue" class="btn btn-primary btn-lg w-100" style="font-size:80px;"/>
            </div>

            <div class="pt-5">
              <input id="GotoClearButton" type="reset" value="ล้างคิว" @click="goToPage3" class="btn btn-secondary btn-sm" />
            </div>

          </div>

          <div v-show="Page2isVisible">
            <div>
              <h2 id="QueueHeaderTxt">หมายเลขคิว</h2>
            </div>
            <div>
              <h1 id="QueueNumber" style="font-size:100px;">{{ queue }}</h1>
            </div>
            <div>
              <h6 id="DateTxt">วันที่ : {{ date }}</h6>
            </div>
            <div>
              <input id="BackButton" type="button" value="กลับไปหน้ารับบัตรคิว" @click="backToPage1" class="btn btn-primary btn-lg" />
            </div>
          </div>

          <div v-show="Page3isVisible">
            <div>
              <input id="ClearButton" type="reset" value="ล้างคิว" @click="clearQueue" class="btn btn-primary btn-lg w-100" />
            </div>
            <div>
              <h3 id="ClearHeaderTxt">หมายเลขคิวปัจจุบัน</h3>
            </div>
            <div>
              <h1 id="QueueNumber" style="font-size:100px;">{{ queue }}</h1>
            </div>
            <div class="pt-3">
              <input id="BackButton" type="button" value="กลับไปหน้ารับบัตรคิว" @click="backToPage1" class="btn btn-primary btn-sm w-100" />
            </div>
          </div>
        </main>
      </div>
    </div>
  </div>
  
  
</template>


<style scoped>
  div {
    padding-bottom: 5px;
  }
</style>
