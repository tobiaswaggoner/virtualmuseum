import fs from "fs";
import path from "path";
import OpenAI from "openai";
import express from "express";

const app = express();
const port = 3000;

app.get('/test', async (req, res) => {
  var response_message = "There was some error here!";
  const openai = new OpenAI();
  try
  {

    const question = req.query.question ?? "What can I ask you?"
    const response = await openai.chat.completions.create({
      messages: [
        { role: "system", content: "You are a helpful assistant." },
        { role: "user", content: question },
      ],
      model: "gpt-3.5-turbo",
    });
    console.log(response.choices[0].message.content);
    response_message = response.choices[0].message.content;
  }
  catch (error)
  {
    console.error(error);
    response_message = error.message;
  }
  try
  {
    const mp3 = await openai.audio.speech.create({
      model: "tts-1",
      voice: "alloy",
      input: response_message,
    });
    const speechFile = path.resolve("./speech.mp3");
    console.log(speechFile);
    const buffer = Buffer.from(await mp3.arrayBuffer());
    await fs.promises.writeFile(speechFile, buffer);
    // return the file
    res.sendFile(speechFile);
  }
  catch (error)
  {
    console.error(error);
    res.status(500).send(error.message);
  }
});
app.use(express.static("public"));
app.listen(port, () => {
  console.log(`Server is running on http://localhost:${port}`);
});